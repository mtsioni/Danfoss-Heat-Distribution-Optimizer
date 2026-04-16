using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services;
using ReactiveUI;

using System.Collections.ObjectModel;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels
{
    using System;

    // small helper class to hold one legend row - the label and color
    public class LegendItem
    {
        public string Title { get; set; } = string.Empty;
        public Avalonia.Media.IBrush Brush { get; set; }
    }

    public class DataVisualizerViewModel : ViewModelBase
    {
        private readonly ResultDataManager _dataManager = new();

        private PlotModel _currentPlotModel = new PlotModel();
        public PlotModel CurrentPlotModel
        {
            get => _currentPlotModel;
            private set => this.RaiseAndSetIfChanged(ref _currentPlotModel, value);
        }

        // legend list - ObservableCollection so UI updates automatically when items change
        private ObservableCollection<LegendItem> _activeLegendItems = new();
        public ObservableCollection<LegendItem> ActiveLegendItems
        {
            get => _activeLegendItems;
            set => this.RaiseAndSetIfChanged(ref _activeLegendItems, value);
        }

        public DataVisualizerViewModel()
        {
            var model = new PlotModel();

            // X-axis: fixed 0-23 (hours), LinearAxis avoids OxyPlot date calculation bugs
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "00",
                Title = "Time (Hours)",
                Key = "TimeAxis",
                MajorStep = 1,
                MinorStep = 1,
                Minimum = 0,
                Maximum = 23,
                MinimumPadding = 0,
                MaximumPadding = 0
            });

            // left Y axis
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Energy (MW, MWh)",
                Key = "LeftAxis",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });

            // right Y axis - used when two categories are active at once
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "",
                Key = "RightAxis",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1,
                Minimum = 0,
                Maximum = 3000
            });

            CurrentPlotModel = model;
        }

        // rebuilds chart every time a checkbox is toggled
        public void UpdatePlot(IEnumerable<DataKind> activeKinds, Period period)
        {
            CurrentPlotModel.Series.Clear();
            ActiveLegendItems.Clear();

            bool hasEnergy = activeKinds.Any(IsEnergy);
            bool hasFinancial = activeKinds.Any(IsFinancial);
            bool hasEnvironment = activeKinds.Any(IsEnvironment);

            // Decide what labels to put on the left and right Y axes
            string leftTitle = "Quantity";
            string rightTitle = "";
            Func<DataKind, string> getAxisKey = k => "LeftAxis";

            if (hasEnergy && hasFinancial)
            {
                leftTitle = "Energy (MW, MWh)";
                rightTitle = "Financial (DKK, DKK/MWh)";
                getAxisKey = k => IsFinancial(k) ? "RightAxis" : "LeftAxis";
            }
            else if (hasEnergy && hasEnvironment)
            {
                leftTitle = "Energy (MW, MWh)";
                rightTitle = GetEnvironmentTitle(activeKinds);
                getAxisKey = k => IsEnvironment(k) ? "RightAxis" : "LeftAxis";
            }
            else if (hasFinancial && hasEnvironment)
            {
                leftTitle = "Financial (DKK, DKK/MWh)";
                rightTitle = GetEnvironmentTitle(activeKinds);
                getAxisKey = k => IsEnvironment(k) ? "RightAxis" : "LeftAxis";
            }
            else if (hasEnergy)
            {
                leftTitle = "Energy (MW, MWh)";
                getAxisKey = k => "LeftAxis";
            }
            else if (hasFinancial)
            {
                leftTitle = "Financial (DKK, DKK/MWh)";
                getAxisKey = k => "LeftAxis";
            }
            else if (hasEnvironment)
            {
                leftTitle = GetEnvironmentTitle(activeKinds);
                getAxisKey = k => "LeftAxis";
            }

            // update axis labels based on what's selected
            var leftAxis = CurrentPlotModel.Axes.FirstOrDefault(a => a.Key == "LeftAxis");
            if (leftAxis != null) leftAxis.Title = leftTitle;

            var rightAxis = CurrentPlotModel.Axes.FirstOrDefault(a => a.Key == "RightAxis");
            if (rightAxis != null) rightAxis.Title = rightTitle;

            foreach (var kind in activeKinds)
            {
                var data = _dataManager.GetTimeSeriesData(kind, period);
                var series = CreateSeries(kind, getAxisKey(kind));

                // extract hour from DateTime so X-axis stays as 0-23 integers
                foreach (var point in data.Values.OrderBy(p => p.Key))
                    series.Points.Add(new DataPoint(point.Key.Hour, point.Value));

                CurrentPlotModel.Series.Add(series);

                // convert OxyPlot color to Avalonia brush for the legend swatch
                var c = series.Color;
                var brush = new Avalonia.Media.SolidColorBrush(
                    Avalonia.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
                ActiveLegendItems.Add(new LegendItem { Title = series.Title, Brush = brush });
            }

            CurrentPlotModel.InvalidatePlot(true);
        }

        // returns correct unit label for environmental axis
        private string GetEnvironmentTitle(IEnumerable<DataKind> activeKinds)
        {
            if (activeKinds.Contains(DataKind.Co2Emissions)) return "CO2 Emissions (kg)";
            if (activeKinds.Contains(DataKind.FuelConsumption)) return "Fuel Consumption (MWh)";
            if (activeKinds.Contains(DataKind.PrimaryEnergy)) return "Primary Energy (MW)";
            return "Environment";
        }

        public void UpdateThemeColors(bool isDarkTheme)
        {
            var textColor = isDarkTheme ? OxyColor.Parse("#efebe5") : OxyColor.Parse("#5E0006");
            var axisColor = isDarkTheme ? OxyColor.Parse("#efebe5") : OxyColor.Parse("#5E0006");

            CurrentPlotModel.TextColor = textColor;
            CurrentPlotModel.PlotAreaBorderColor = axisColor;

            foreach (var axis in CurrentPlotModel.Axes)
            {
                axis.TextColor = textColor;
                axis.TitleColor = textColor;
                axis.TicklineColor = axisColor;
                axis.AxislineColor = axisColor;
            }

            CurrentPlotModel.InvalidatePlot(false);
        }

        private LineSeries CreateSeries(DataKind kind, string yAxisKey)
        {
            var series = new LineSeries
            {
                Title = SplitCamelCase(kind.ToString()),
                StrokeThickness = 2,
                Color = GetColor(kind),
                YAxisKey = yAxisKey
            };

            return series;
        }

        private bool IsEnergy(DataKind kind) => kind switch
        {
            DataKind.HeatProduced => true,
            DataKind.HeatConsumed => true,
            DataKind.ElectricityProduced => true,
            DataKind.ElectricityConsumed => true,
            _ => false
        };

        private bool IsFinancial(DataKind kind) => kind switch
        {
            DataKind.ElectricityPrice => true,
            DataKind.MoneyEarned => true,
            DataKind.MoneySpent => true,
            DataKind.Profit => true,
            DataKind.Expenses => true,
            _ => false
        };

        private bool IsEnvironment(DataKind kind) => kind switch
        {
            DataKind.Co2Emissions => true,
            DataKind.FuelConsumption => true,
            DataKind.PrimaryEnergy => true,
            _ => false
        };

        // "HeatProduced" -> "Heat Produced"
        private string SplitCamelCase(string s)
        {
            return System.Text.RegularExpressions.Regex.Replace(s, "([A-Z])", " $1").Trim();
        }

        private OxyColor GetColor(DataKind kind) => kind switch
        {
            DataKind.HeatProduced => OxyColors.Red,
            DataKind.HeatConsumed => OxyColors.Black,
            DataKind.ElectricityProduced => OxyColors.Blue,
            DataKind.ElectricityConsumed => OxyColors.LightBlue,
            DataKind.ElectricityPrice => OxyColors.DarkGoldenrod,
            DataKind.MoneyEarned => OxyColors.Green,
            DataKind.MoneySpent => OxyColors.DarkGreen,
            DataKind.Profit => OxyColors.LimeGreen,
            DataKind.Expenses => OxyColors.DarkRed,
            DataKind.Co2Emissions => OxyColors.Gray,
            DataKind.FuelConsumption => OxyColors.Brown,
            DataKind.PrimaryEnergy => OxyColors.SaddleBrown,
            _ => OxyColors.Gray
        };
    }
}