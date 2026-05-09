using System;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class HybridUnit : OptimizedUnit, IElectricUnit, IFueledUnit
    {
        // From IElectricUnit
        public double MaxElectricity { get; set; }

        // From IFueledUnit  
        public string FuelName { get; set; }
        public double FuelConsumption { get; set; }
        public double Emissions { get; set; }

        public HybridUnit(int unitID, string name, double maxElectricity, string fuelName, 
        double fuelConsumption, double emissions, double maxHeat, double productionCost)
        {
            UnitID = unitID;
            Name = name;
            MaxElectricity = maxElectricity;
            FuelName = fuelName;
            FuelConsumption = fuelConsumption;
            Emissions = emissions;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            ProductionCostRecords = new();
            HeatRecords = new();
            ElectricityRecords = new();
            PollutionRecords = new();
            FuelConsumptionRecords = new();
            HeatPerPriceRecords = new(); 
        }
        public override double CalculateNetProductionCost(double electricityPrice)
        {
            return double.Round(ProductionCost - MaxElectricity * electricityPrice, 2);
        }
        public override void UpdateRecords(double workload, double netProductionCost, DateTime hour)
        {
            //Update ProductionCostRecords
            if (ProductionCostRecords.Values.ContainsKey(hour))
            {
                ProductionCostRecords[hour] = netProductionCost * workload;
            }
            else
            {
                ProductionCostRecords.Values.Add(hour, netProductionCost * workload);
            }
            //Update HeatRecords
            if (HeatRecords.Values.ContainsKey(hour))
            {
                HeatRecords[hour] = MaxHeat * workload;
            }
            else
            {
                HeatRecords.Values.Add(hour, MaxHeat * workload);
            }
            //Update ElectricityRecords
            if (ElectricityRecords.Values.ContainsKey(hour))
            {
                ElectricityRecords[hour] = MaxElectricity * workload;
            }
            else
            {
                ElectricityRecords.Values.Add(hour, MaxElectricity * workload);
            }
            //Update PollutionRecords
            if (PollutionRecords.Values.ContainsKey(hour))
            {
                PollutionRecords[hour] = Emissions * workload;
            }
            else
            {
                PollutionRecords.Values.Add(hour, Emissions * workload);
            }
            // Update FuelConsumptionRecords
            if (FuelConsumptionRecords.Values.ContainsKey(hour))
            {
                FuelConsumptionRecords[hour] = FuelConsumption * workload;
            }
            else
            {
                FuelConsumptionRecords.Values.Add(hour, FuelConsumption * workload);
            }
        }
    }
}