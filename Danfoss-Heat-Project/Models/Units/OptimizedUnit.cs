using System;
namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public abstract class OptimizedUnit : Unit
    {
        public TimeSeries<double> ProductionCostRecords { get; set; } = new();
        public TimeSeries<double> HeatRecords { get; set; } = new();
        public TimeSeries<double> ElectricityRecords { get; set; } = new();
        public TimeSeries<double> PollutionRecords { get; set; } = new();
        public TimeSeries<double> FuelConsumptionRecords { get; set; } = new();
        public TimeSeries<double> HeatPerPriceRecords {get; set;} = new();
        public abstract double CalculateNetProductionCost(double electricityPrice);
        public abstract void UpdateRecords(double workload, double netProductionCost, DateTime hour);
    }
}