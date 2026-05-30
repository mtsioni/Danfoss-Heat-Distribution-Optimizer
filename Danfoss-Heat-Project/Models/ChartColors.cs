using OxyPlot;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public static class ChartColors
    {
        public static readonly OxyColor[] CoreColors = new[] 
        { 
            OxyColor.FromRgb(230, 25, 75),   // Red
            OxyColor.FromRgb(0, 130, 200),   // Blue
            OxyColor.FromRgb(60, 180, 75),   // Green
            OxyColor.FromRgb(245, 130, 48),  // Orange
            OxyColor.FromRgb(145, 30, 180),  // Purple
            OxyColor.FromRgb(255, 225, 25),  // Yellow
            OxyColor.FromRgb(70, 240, 240),  // Cyan
            OxyColor.FromRgb(240, 50, 230),  // Magenta
            OxyColor.FromRgb(210, 245, 60),  // Lime
            OxyColor.FromRgb(0, 128, 128)    // Teal
        };

        public static OxyColor GetColor(DataKind kind) => kind switch
        {
            DataKind.HeatDemand => OxyColors.Red,
            DataKind.HeatProduction => OxyColors.Transparent,
            DataKind.Electricity => OxyColor.FromRgb(255, 255, 0),
            DataKind.ElectricityPrice => OxyColors.Cyan,
            DataKind.Co2Emissions => OxyColors.Black,
            DataKind.FuelConsumption => OxyColor.FromRgb(139, 69, 19),
            DataKind.ProductionCosts => OxyColors.Green,
            _ => OxyColors.Gray
        };
    }
}
