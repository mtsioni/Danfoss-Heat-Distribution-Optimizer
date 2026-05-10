using OxyPlot;
using OxyPlot.Axes;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public static class Axes
    {
        public static PlotModel CreateAxes()
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

            return model;
        }
    }
}