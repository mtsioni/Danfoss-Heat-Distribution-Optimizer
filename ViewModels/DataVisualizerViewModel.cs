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

        private ObservableCollection<LegendItem> _activeLegendItems = new();
        public ObservableCollection<LegendItem> ActiveLegendItems
        {
            get => _activeLegendItems;
            set => this.RaiseAndSetIfChanged(ref _activeLegendItems, value);
        }

        public DataVisualizerViewModel()
        {
            var model = new PlotModel { Title = "Unit Optimization Data Overlay" };

            // Axes are created once and never recreated - this prevents the chart from rescaling on every toggle
            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "HH:mm",
                Title = "Time",
                Key = "TimeAxis"
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Quantity / Energy (MW, MWh, kg)",
                Key = "LeftAxis",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Financial (DKK, DKK/MWh)",
                Key = "RightAxis",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1,
                // Reserve space always so the chart width stays stable even without financial series
                Minimum = 0,
                Maximum = 3000
            });

            CurrentPlotModel = model;
        }

        public void UpdatePlot(IEnumerable<DataKind> activeKinds, Period period)
        {
            // Only clear series - axes stay untouched so the chart never shifts
            CurrentPlotModel.Series.Clear();
            ActiveLegendItems.Clear();

            bool hasEnergy = activeKinds.Any(IsEnergy);
            bool hasFinancial = activeKinds.Any(IsFinancial);
            bool hasEnvironment = activeKinds.Any(IsEnvironment);

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
                rightTitle = "Environment (kg, ton)";
                getAxisKey = k => IsEnvironment(k) ? "RightAxis" : "LeftAxis";
            }
            else if (hasFinancial && hasEnvironment)
            {
                leftTitle = "Financial (DKK, DKK/MWh)";
                rightTitle = "Environment (kg, ton)";
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
                leftTitle = "Environment (kg, ton)";
                getAxisKey = k => "LeftAxis";
            }

            var leftAxis = CurrentPlotModel.Axes.FirstOrDefault(a => a.Key == "LeftAxis");
            if (leftAxis != null) leftAxis.Title = leftTitle;

            var rightAxis = CurrentPlotModel.Axes.FirstOrDefault(a => a.Key == "RightAxis");
            if (rightAxis != null) rightAxis.Title = rightTitle;

            foreach (var kind in activeKinds)
            {
                var data = _dataManager.GetTimeSeriesData(kind, period);
                var series = CreateSeries(kind, getAxisKey(kind));

                foreach (var point in data.Values.OrderBy(p => p.Key))
                    series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.Key), point.Value));

                CurrentPlotModel.Series.Add(series);
                
                var c = series.Color;
                var brush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
                ActiveLegendItems.Add(new LegendItem { Title = series.Title, Brush = brush });
            }

            CurrentPlotModel.InvalidatePlot(true);
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