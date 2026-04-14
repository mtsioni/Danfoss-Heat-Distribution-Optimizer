using Avalonia.Rendering.Composition;
using Danfoss_Heat_Distribution_Optimizer.Models;
namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IOptimizedUnit : IUnit, IHeatUnit
    {
        double? NetProductionCost { get; set; }
        double? NetHeat { get; set; }
        double? NetElectricity { get; set; }
        double? NetPollution { get; set; }
        TimeSeries<double>? ProductionCostRecords { get; set; }
        TimeSeries<double>? HeatRecords { get; set; }
        TimeSeries<double>? ElectricityRecords { get; set; }
        TimeSeries<double>? PollutionRecords { get; set; }
        TimeSeries<double>? HeatPerPriceRecords {get; set;}


    }
}