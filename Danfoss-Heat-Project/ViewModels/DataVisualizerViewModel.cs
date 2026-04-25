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
using System;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels
{
    public class LegendItem
    {
        public string Title { get; set; } = string.Empty;
        public Avalonia.Media.IBrush? Brush { get; set; }
    }

    public class DataVisualizerViewModel : ViewModelBase
    {
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

        private bool _isLegendExpanded = false;
        public bool IsLegendExpanded
        {
            get => _isLegendExpanded;
            set => this.RaiseAndSetIfChanged(ref _isLegendExpanded, value);
        }

        public void ToggleLegend()
        {
            IsLegendExpanded = !IsLegendExpanded;
        }

        public DataVisualizerModel Model { get; } = new();

        public DataVisualizerViewModel()
        {
            var model = new PlotModel();

            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd/MM",
                Title = "Date",
                Key = "TimeAxis",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalType = DateTimeIntervalType.Days,
                MinorIntervalType = DateTimeIntervalType.Hours,
                IsPanEnabled = false,
                IsZoomEnabled = false
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Energy (MW, MWh)",
                Key = "LeftAxis",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                MajorStep = 1,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "",
                Key = "RightAxis",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1,
                IsAxisVisible = false,
                IsPanEnabled = false,
                IsZoomEnabled = false
            });

            CurrentPlotModel = model;
        }

        public void UpdatePlot(IEnumerable<DataKind> activeKinds, Period period)
        {
            CurrentPlotModel.Series.Clear();
            ActiveLegendItems.Clear();

            var activeGroups = new List<string>();
            bool hasEnergy = activeKinds.Any(k => k == DataKind.HeatDemand || k == DataKind.HeatProduction || k == DataKind.Electricity);
            bool hasFinance = activeKinds.Any(k => k == DataKind.ProductionCosts || k == DataKind.ElectricityPrice);
            bool hasCo2 = activeKinds.Any(k => k == DataKind.Co2Emissions);
            bool hasFuel = activeKinds.Any(k => k == DataKind.FuelConsumption);

            if (hasEnergy) activeGroups.Add("Energy (MW, MWh)");
            if (hasFinance) activeGroups.Add("Financial (DKK)");
            if (hasCo2) activeGroups.Add("CO2 Emissions (kg)");
            if (hasFuel) activeGroups.Add("Fuel Consumption (MWh)");

            string leftTitle = "";
            string rightTitle = "";
            Func<DataKind, string> getAxisKey = k => "LeftAxis";

            if (activeGroups.Count == 1)
            {
                leftTitle = activeGroups[0];
            }
            else if (activeGroups.Count == 2)
            {
                leftTitle = activeGroups[0];
                rightTitle = activeGroups[1];
                
                getAxisKey = k => 
                {
                    string group = "";
                    if (k == DataKind.HeatDemand || k == DataKind.HeatProduction || k == DataKind.Electricity) group = "Energy (MW, MWh)";
                    else if (k == DataKind.ProductionCosts || k == DataKind.ElectricityPrice) group = "Financial (DKK)";
                    else if (k == DataKind.Co2Emissions) group = "CO2 Emissions (kg)";
                    else if (k == DataKind.FuelConsumption) group = "Fuel Consumption (MWh)";

                    return group == activeGroups[1] ? "RightAxis" : "LeftAxis";
                };
            }

            var timeAxis = CurrentPlotModel.Axes.FirstOrDefault(a => a.Key == "TimeAxis") as DateTimeAxis;
            if (timeAxis != null) 
            {
                if (period == Period.Winter)
                {
                    timeAxis.Minimum = DateTimeAxis.ToDouble(new DateTime(2026, 1, 5));
                    timeAxis.Maximum = DateTimeAxis.ToDouble(new DateTime(2026, 1, 19));
                }
                else
                {
                    timeAxis.Minimum = DateTimeAxis.ToDouble(new DateTime(2025, 9, 8));
                    timeAxis.Maximum = DateTimeAxis.ToDouble(new DateTime(2025, 9, 22));
                }
            }

            var leftAxis = CurrentPlotModel.Axes.FirstOrDefault(a => a.Key == "LeftAxis") as LinearAxis;
            if (leftAxis != null) 
            {
                leftAxis.Title = leftTitle;
                leftAxis.IsAxisVisible = !string.IsNullOrEmpty(leftTitle);
                leftAxis.Minimum = double.NaN; // Auto-scale (allow negative)
                leftAxis.MajorStep = leftTitle.Contains("Energy") ? 1 : double.NaN;
            }

            var rightAxis = CurrentPlotModel.Axes.FirstOrDefault(a => a.Key == "RightAxis") as LinearAxis;
            if (rightAxis != null) 
            {
                rightAxis.Title = rightTitle;
                rightAxis.IsAxisVisible = !string.IsNullOrEmpty(rightTitle);
                rightAxis.Minimum = double.NaN; // Auto-scale (allow negative)
                rightAxis.MajorStep = double.NaN;
            }

            // 1. First add the bars (to the back)
            if (activeKinds.Contains(DataKind.HeatProduction))
            {
                AddStackedHeatProduction(period, getAxisKey(DataKind.HeatProduction));
            }

            // 2. Then add the lines (to the front)
            foreach (var kind in activeKinds.Where(k => k != DataKind.HeatProduction))
            {
                var data = GetAggregatedData(kind, period);
                var series = CreateSeries(kind, getAxisKey(kind));

                foreach (var point in data.OrderBy(p => p.Key))
                {
                    // Align points to center of bar (minute 30) for visual stability
                    series.Points.Add(DateTimeAxis.CreateDataPoint(point.Key.AddMinutes(30), point.Value));
                }

                CurrentPlotModel.Series.Add(series);

                var c = series.Color;
                var brush = new Avalonia.Media.SolidColorBrush(
                    Avalonia.Media.Color.FromArgb(c.A, c.R, c.G, c.B));
                ActiveLegendItems.Add(new LegendItem { Title = series.Title, Brush = brush });
            }

            CurrentPlotModel.InvalidatePlot(true);
        }

        private void AddStackedHeatProduction(Period period, string yAxisKey)
        {
            var units = ResultDataManager.GetResultData();
            if (units == null || units.Count == 0) return;

            bool isCorrectPeriod(DateTime dt) => period == Period.Winter ? dt.Month == 1 : dt.Month == 9;

            var timeStamps = units.SelectMany(u => u.HeatRecords.Values.Keys)
                                 .Where(isCorrectPeriod)
                                 .Distinct()
                                 .OrderBy(t => t)
                                 .ToList();

            if (timeStamps.Count == 0) return;

            var lastY = timeStamps.ToDictionary(t => t, t => 0.0);
            int colorIndex = 0;
            // Unit palette (avoids conflict with metric lines)
            var coreColors = new[] 
            { 
                OxyColor.FromRgb(255, 105, 180), // Hot Pink
                OxyColor.FromRgb(0, 191, 255),   // Deep Sky Blue
                OxyColor.FromRgb(186, 85, 211),  // Medium Orchid
                OxyColor.FromRgb(0, 206, 209),   // Dark Turquoise
                OxyColor.FromRgb(123, 104, 238), // Medium Slate Blue
                OxyColor.FromRgb(255, 0, 255),   // Magenta
                OxyColor.FromRgb(175, 238, 238), // Pale Turquoise
                OxyColor.FromRgb(218, 112, 214), // Orchid
                OxyColor.FromRgb(70, 130, 180),  // Steel Blue
                OxyColor.FromRgb(221, 160, 221)  // Plum
            };

            foreach (var unit in units)
            {
                var color = coreColors[colorIndex % coreColors.Length];
                var series = new RectangleBarSeries { Title = unit.Name, FillColor = color, YAxisKey = yAxisKey, StrokeThickness = 0 };

                foreach (var kvp in unit.HeatRecords.Values)
                {
                    var dt = kvp.Key;
                    var val = kvp.Value;

                    if (isCorrectPeriod(dt) && val > 0)
                    {
                        double y0 = lastY[dt];
                        double y1 = y0 + val;
                        // Center around minute 30, with a narrow 12-minute span (20%)
                        double x0 = DateTimeAxis.ToDouble(dt.AddMinutes(24));
                        double x1 = DateTimeAxis.ToDouble(dt.AddMinutes(36));

                        series.Items.Add(new RectangleBarItem(x0, y0, x1, y1));
                        lastY[dt] = y1;
                    }
                }

                if (series.Items.Count > 0)
                {
                    CurrentPlotModel.Series.Add(series);
                    var c = series.FillColor;
                    ActiveLegendItems.Add(new LegendItem 
                    { 
                        Title = series.Title, 
                        Brush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromArgb(c.A, c.R, c.G, c.B)) 
                    });
                }
                
                colorIndex++;
            }
        }

        public Dictionary<DateTime, double> GetAggregatedData(DataKind kind, Period period)
        {
            var dict = new Dictionary<DateTime, double>();
            bool isCorrectPeriod(DateTime dt) => period == Period.Winter ? dt.Month == 1 : dt.Month == 9;

            if (kind == DataKind.ElectricityPrice)
            {
                try 
                {
                    var priceSeries = SourceDataManager.GetElectricityPrice();
                    foreach (var kvp in priceSeries.Values)
                    {
                        if (isCorrectPeriod(kvp.Key))
                            dict[kvp.Key] = kvp.Value;
                    }
                } 
                catch { }
                return dict;
            }
            
            if (kind == DataKind.HeatDemand)
            {
                try 
                {
                    var demandSeries = SourceDataManager.GetHeatDemand();
                    foreach (var kvp in demandSeries.Values)
                    {
                        if (isCorrectPeriod(kvp.Key))
                            dict[kvp.Key] = kvp.Value;
                    }
                } 
                catch { }
                return dict;
            }

            TimeSeries<double> targetSeries = kind switch
            {
                DataKind.Electricity => Model.ElectricityProduced,
                DataKind.Co2Emissions => Model.Co2Emissions,
                DataKind.FuelConsumption => Model.FuelConsumption,
                DataKind.ProductionCosts => Model.ProductionCostRecords,
                _ => new TimeSeries<double>()
            };

            if (targetSeries != null && targetSeries.Values != null)
            {
                foreach (var kvp in targetSeries.Values)
                {
                    if (isCorrectPeriod(kvp.Key))
                        dict[kvp.Key] = kvp.Value;
                }
            }

            return dict;
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
                XAxisKey = "TimeAxis",
                YAxisKey = yAxisKey,
                StrokeThickness = 4,
                Color = GetColor(kind),
                MarkerType = MarkerType.Circle,
                MarkerSize = 2
            };

            return series;
        }

        private string SplitCamelCase(string s)
        {
            return System.Text.RegularExpressions.Regex.Replace(s, "([A-Z])", " $1").Trim();
        }

        private OxyColor GetColor(DataKind kind) => kind switch
        {
            DataKind.HeatDemand => OxyColors.Red,
            DataKind.HeatProduction => OxyColors.Transparent, // Not used for lines
            DataKind.Electricity => OxyColor.FromRgb(255, 255, 0), // Bright Yellow
            DataKind.ElectricityPrice => OxyColors.Cyan,
            DataKind.Co2Emissions => OxyColors.Black,
            DataKind.FuelConsumption => OxyColor.FromRgb(139, 69, 19), // Brown
            DataKind.ProductionCosts => OxyColors.Green,
            _ => OxyColors.Gray
        };
    }
}