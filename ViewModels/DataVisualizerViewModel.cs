using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services;
using ReactiveUI;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels
{
    public class DataVisualizerViewModel : ViewModelBase
    {
        private readonly ResultDataManager _dataManager = new();

        private PlotModel _currentPlotModel = new PlotModel();
        public PlotModel CurrentPlotModel
        {
            get => _currentPlotModel;
            private set => this.RaiseAndSetIfChanged(ref _currentPlotModel, value);
        }

        public DataVisualizerViewModel()
        {
            var model = new PlotModel { Title = "Unit Optimization Data Overlay" };

            model.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopRight,
                LegendPlacement = LegendPlacement.Inside,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBackground = OxyColor.FromArgb(180, 240, 230, 210),
                LegendBorder = OxyColor.FromArgb(100, 90, 0, 6),
                LegendBorderThickness = 1,
                LegendFontSize = 11
            });

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

            foreach (var kind in activeKinds)
            {
                var data = _dataManager.GetTimeSeriesData(kind, period);
                var series = CreateSeries(kind);

                foreach (var point in data.Values.OrderBy(p => p.Key))
                    series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.Key), point.Value));

                CurrentPlotModel.Series.Add(series);
            }

            CurrentPlotModel.InvalidatePlot(true);
        }

        private LineSeries CreateSeries(DataKind kind)
        {
            var series = new LineSeries
            {
                Title = SplitCamelCase(kind.ToString()),
                StrokeThickness = 2,
                Color = GetColor(kind),
                YAxisKey = IsFinancial(kind) ? "RightAxis" : "LeftAxis"
            };

            if (kind == DataKind.HeatConsumed)
                series.StrokeThickness = 4;

            return series;
        }

        private bool IsFinancial(DataKind kind) => kind switch
        {
            DataKind.ElectricityPrice => true,
            DataKind.MoneyEarned => true,
            DataKind.MoneySpent => true,
            DataKind.Profit => true,
            DataKind.Expenses => true,
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