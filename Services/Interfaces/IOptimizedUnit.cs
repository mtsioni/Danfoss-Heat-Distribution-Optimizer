using Danfoss_Heat_Distribution_Optimizer.Models;
namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IOptimizedUnit
    {
        double NetProductionCost { get; set; }
        double NetHeat { get; set; }
        double NetElectricity { get; set; }
        double NetPollution { get; set; }
        TimeSeries<double> ProductionCost { get; set; }
        TimeSeries<double> Heat { get; set; }
        TimeSeries<double> Electricity { get; set; }
        TimeSeries<double> Pollution { get; set; }
    }
}