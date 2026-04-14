using System;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class CombustionUnit : IUnit, IFueledUnit, IHeatUnit, IOptimizedUnit
    {
        // From IUnit
        public int UnitID { get; set; }
        public string Name { get; set; }

        // From IFueledUnit  
        public string FuelName { get; set; }
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
        public TimeSeries<double> HeatPerPriceRecords {get; set;}



        public CombustionUnit(int unitID, string name,
                                string fuelName, double fuelConsumption, double emissions,
                                double maxHeat, double productionCost)
        {
            UnitID = unitID;
            Name = name;

            FuelName = fuelName;
            FuelConsumption = fuelConsumption;
            Emissions = emissions;

            MaxHeat = maxHeat;
            ProductionCost = productionCost;

            NetProductionCost = 0;
            NetHeat = 0;
            NetElectricity = 0;
            NetPollution = 0;
            ProductionCostRecords = new();
            HeatRecords = new();
            ElectricityRecords = new();
            PollutionRecords = new();
            HeatPerPriceRecords = new(); 
        }
        public double CalculateTotalProductionCost(double electricityPrice)
        {
            return ProductionCost;
        }
        public void UpdateRecords(double electricityPrice, DateTime hour)
        {
            //Update ProductionCostRecords
            if (ProductionCostRecords.Values.ContainsKey(hour))
            {
                ProductionCostRecords[hour] = CalculateTotalProductionCost(electricityPrice);
            }
            else
            {
                ProductionCostRecords.Values.Add(hour, CalculateTotalProductionCost(electricityPrice));
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
                ElectricityRecords[hour] = 0;
            }
            else
            {
                ElectricityRecords.Values.Add(hour, 0);
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
            
            // Update NetProductionCost
            if (NetProductionCost == 0)
            {
                NetProductionCost = ProductionCostRecords[hour];
            }
            else
            {
                NetProductionCost = (NetProductionCost + ProductionCostRecords[hour]) / 2;
            }
            // Update NetHeat
            if (NetHeat == 0)
            {
                NetHeat = HeatRecords[hour];
            }
            else
            {
                NetHeat = (NetHeat + HeatRecords[hour]) / 2;
            }
            // Update NetPollution
            if (NetPollution == 0)
            {
                NetPollution = PollutionRecords[hour];
            }
            else
            {
                NetPollution = (NetPollution + PollutionRecords[hour]) / 2;
            }
        }
    }
}