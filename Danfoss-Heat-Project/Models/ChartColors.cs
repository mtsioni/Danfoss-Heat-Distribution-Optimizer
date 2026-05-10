using OxyPlot;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public static class ChartColors
    {
        public static readonly OxyColor[] CoreColors = new[] 
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

        public static OxyColor GetColor(DataKind kind) => kind switch
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
