using System;
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
        public double NetProductionCost { get; set; }
        public double NetHeat { get; set; }
        public double NetElectricity { get; set; }
        public double NetPollution { get; set; }
        public TimeSeries<double> ProductionCostRecords { get; set; }
        public TimeSeries<double> HeatRecords { get; set; }
        public TimeSeries<double> ElectricityRecords { get; set; }
        public TimeSeries<double> PollutionRecords { get; set; }
        public TimeSeries<double> HeatPerPriceRecords {get; set;}

        // From IElectricUnit
        public double MaxElectricity { get; set; }

        public ElectricUnit(int unitID, string name, double maxElectricity, double maxHeat, 
        double productionCost)
        {
            UnitID = unitID;
            Name = name;
            MaxElectricity = maxElectricity;
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
            return ProductionCost - MaxElectricity * electricityPrice;
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
                ElectricityRecords[hour] = MaxElectricity;
            }
            else
            {
                ElectricityRecords.Values.Add(hour, MaxElectricity);
            }
            //Update PollutionRecords
            if (PollutionRecords.Values.ContainsKey(hour))
            {
                PollutionRecords[hour] = 0;
            }
            else
            {
                PollutionRecords.Values.Add(hour, 0);
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
            // Update NetElectricity
            if (NetElectricity == 0)
            {
                NetElectricity = HeatRecords[hour];
            }
            else
            {
                NetElectricity = (NetElectricity + ElectricityRecords[hour]) / 2;
            }
        }
    }
}