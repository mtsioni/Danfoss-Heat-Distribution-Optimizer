using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class ElectricUnit : IUnit, IHeatUnit, IOptimizedUnit, IElectricUnit
    {
        // From IUnit
        public int UnitID { get; set; }
        public string Name { get; set; }

        // From IHeatUnit
        public double MaxHeat { get; set; }
        public double ProductionCost { get; set; }

        // From IOptimizedUnit
        public double? NetProductionCost { get; set; }
        public double? NetHeat { get; set; }
        public double? NetElectricity { get; set; }
        public double? NetPollution { get; set; }
        public TimeSeries<double>? ProductionCostRecords { get; set; }
        public TimeSeries<double>? HeatRecords { get; set; }
        public TimeSeries<double>? ElectricityRecords { get; set; }
        public TimeSeries<double>? PollutionRecords { get; set; }

        // From IElectricUnit
        public double? MaxElectricity { get; set; }

        public ElectricUnit(int unitID, string name, double? maxElectricity, double maxHeat, 
        double productionCost)
        {
            UnitID = unitID;
            Name = name;
            MaxElectricity = maxElectricity;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            NetProductionCost = null;
            NetHeat = null;
            NetElectricity = null;
            NetPollution = null;
            ProductionCostRecords = null;
            HeatRecords = null;
            ElectricityRecords = null;
            PollutionRecords = null;
        }
    }
}