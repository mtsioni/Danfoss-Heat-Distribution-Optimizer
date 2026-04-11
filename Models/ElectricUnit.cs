using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class ElectricUnit : IUnit, IHeatUnit, IOptimizedUnit, IElectricUnit
    {
        // From IUnit
        public int UnitID { get; set; }
        public required string Name { get; set; }

        // From IHeatUnit
        public double MaxHeat { get; set; }
        public double ProductionCost { get; set; }

        // From IOptimizedUnit
        public double NetProductionCost { get; set; }
        public double NetHeat { get; set; }
        public double NetElectricity { get; set; }
        public double NetPollution { get; set; }
        public TimeSeries<double> ProductionCostRecords { get; set; }
        public TimeSeries<double> HeatRecords { get; set; }
        public TimeSeries<double> ElectricityRecords { get; set; }
        public TimeSeries<double> PollutionRecords { get; set; }

        // From IElectricUnit
        public double MaxElectricity { get; set; }
    }

}