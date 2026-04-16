using System;
using Danfoss_Heat_Distribution_Optimizer.Models;
namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IOptimizedUnit : IUnit, IHeatUnit
    {
        TimeSeries<double> ProductionCostRecords { get; set; }
        TimeSeries<double> HeatRecords { get; set; }
        TimeSeries<double> ElectricityRecords { get; set; }
        TimeSeries<double> PollutionRecords { get; set; }
        TimeSeries<double> FuelConsumptionRecords { get; set; }
        TimeSeries<double> HeatPerPriceRecords {get; set;}
        double CalculateNetProductionCost(double electricityPrice);
        void UpdateRecords(double electricityPrice, DateTime hour);
    }
}