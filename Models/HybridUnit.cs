using System;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class HybridUnit : IUnit, IElectricUnit, IFueledUnit, IHeatUnit, IOptimizedUnit
    {
        // From IUnit
        public int UnitID { get; set; }
        public string Name { get; set; }

        // From IElectricUnit
        public double MaxElectricity { get; set; }

        // From IFueledUnit  
        public string FuelName { get; set; }
        public double FuelConsumption { get; set; }
        public double Emissions { get; set; }

        // From IHeatUnit
        public double MaxHeat { get; set; }
        public double ProductionCost { get; set; }

        // From IOptimizedUnit
        public TimeSeries<double> ProductionCostRecords { get; set; }
        public TimeSeries<double> HeatRecords { get; set; }
        public TimeSeries<double> ElectricityRecords { get; set; }
        public TimeSeries<double> PollutionRecords { get; set; }
        public TimeSeries<double> FuelConsumptionRecords { get; set; }
        public TimeSeries<double> HeatPerPriceRecords {get; set;}

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
        public double CalculateNetProductionCost(double electricityPrice)
        {
            return ProductionCost - MaxElectricity * electricityPrice;
        }
        public void UpdateRecords(double electricityPrice, DateTime hour)
        {
            //Update ProductionCostRecords
            if (ProductionCostRecords.Values.ContainsKey(hour))
            {
                ProductionCostRecords[hour] = CalculateNetProductionCost(electricityPrice);
            }
            else
            {
                ProductionCostRecords.Values.Add(hour, CalculateNetProductionCost(electricityPrice));
            }
            //Update HeatRecords
            if (HeatRecords.Values.ContainsKey(hour))
            {
                HeatRecords[hour] = MaxHeat;
            }
            else
            {
                HeatRecords.Values.Add(hour, MaxHeat);
            }
            //Update ElectricityRecords
            if (ElectricityRecords.Values.ContainsKey(hour))
            {
                ElectricityRecords[hour] = MaxElectricity;
            }
            else
            {
                ElectricityRecords.Values.Add(hour, MaxElectricity);
            }
            //Update PollutionRecords
            if (PollutionRecords.Values.ContainsKey(hour))
            {
                PollutionRecords[hour] = Emissions;
            }
            else
            {
                PollutionRecords.Values.Add(hour, Emissions);
            }
            // Update FuelConsumptionRecords
            if (FuelConsumptionRecords.Values.ContainsKey(hour))
            {
                FuelConsumptionRecords[hour] = FuelConsumption;
            }
            else
            {
                FuelConsumptionRecords.Values.Add(hour, FuelConsumption);
            }
        }
    }
}