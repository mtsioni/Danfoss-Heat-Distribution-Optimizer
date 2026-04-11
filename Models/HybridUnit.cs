using System.Reactive;
using System.Reflection.PortableExecutable;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class HybridUnit : IUnit, IElectricUnit, IFueledUnit, IHeatUnit, IOptimizedUnit
    {
        // From IUnit
        public int UnitID { get; set; }
        public required string Name { get; set; }

        // From IElectricUnit
        public double MaxElectricity { get; set; }

        // From IFueledUnit  
        public required string FuelName { get; set; }
        public double FuelConsumption { get; set; }
        public double Emissions { get; set; }

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

        public HybridUnit(int unitID, string name, double maxElectricity, string fuelName, 
        double fuelConsumption, double emissions, double maxHeat, double productionCost, 
        double netProductionCost, double netHeat, double netElectricity, double netPollution, 
        TimeSeries<double> productionCostRecords, TimeSeries<double> heatRecords, 
        TimeSeries<double> electricityRecords, TimeSeries<double> pollutionRecords)
        {
            UnitID = unitID;
            Name = name;
            MaxElectricity = maxElectricity;
            FuelName = fuelName;
            FuelConsumption = fuelConsumption;
            Emissions = emissions;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            NetProductionCost = netProductionCost;
            NetHeat = netHeat;
            NetElectricity = netElectricity;
            NetPollution = netPollution;
            ProductionCostRecords = productionCostRecords;
            HeatRecords = heatRecords;
            ElectricityRecords = electricityRecords;
            PollutionRecords = pollutionRecords;
        }
    }
}