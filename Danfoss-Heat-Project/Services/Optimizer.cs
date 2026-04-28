using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class Optimizer
    {
        public static DateTime OptimizationPeriodStart { get; set; }
        public static DateTime OptimizationPeriodEnd { get; set; }
        public static TimeSeries<double>? ElectricityPrices { get; set; }
        public static TimeSeries<double>? HeatDemand { get; set; }
        public static int TimeResolution { get; set; } = 1;
        
        public static string MaintainedUnitName { get; set; } = "";
        public static DateTime WinterMaintenanceStart { get; set; }
        public static DateTime SummerMaintenanceStart { get; set; }
        public static double MaintenanceLength { get; set; }
        private static List<IOptimizedUnit> _availableUnits = new();


        
        private static void RetrieveUnits()
        {
            // May need to initialize
            _availableUnits = AssetManager.GetDataForOptimizer();
        }
        

        public static void SetMaintenanceData(string unitName, double hours)
        {
            MaintainedUnitName = unitName;
            MaintenanceLength = hours;
        }

        private static DateTime GetSummerMaintenanceStart()
        {
            Random genSsummer= new Random();
            
            DateTime SummerStartRange = new DateTime(2025, 9, 8); 
            DateTime SummerEndRange = new DateTime(2025, 9, 22); 
            
            long SummerRange = SummerEndRange.Ticks - SummerStartRange.Ticks;
            
            long randomSummerTicks = (long)(genSsummer.NextDouble() * SummerRange);
            
            SummerMaintenanceStart = SummerStartRange.AddTicks(randomSummerTicks);
            
            return SummerMaintenanceStart;
        }

        private static DateTime GetWinterMaintenanceStart()
        {
            Random genWinter = new Random();
            
            DateTime WinterStartRange = new DateTime(2026, 1, 5); 
            DateTime WinterEndRange = new DateTime(2026, 1, 19); 
            
            long WinterRange = WinterEndRange.Ticks - WinterStartRange.Ticks;
            
            long randomWinterTicks = (long)(genWinter.NextDouble() * WinterRange);
            
            WinterMaintenanceStart = WinterStartRange.AddTicks(randomWinterTicks);
            
            return WinterMaintenanceStart;
        }

        private static void RetrieveSourceData()
        {

            ElectricityPrices = SourceDataManager.GetElectricityPrice();
            HeatDemand = SourceDataManager.GetHeatDemand();

        }
        private static void CalculateHeatPerPrice(DateTime hour, double electricityPrice)
        {
            double heatPerPriceCrude; 
            double heatPerPrice;
            for (int i = 0; i < _availableUnits.Count; i++)
            {    
                heatPerPriceCrude = _availableUnits[i].MaxHeat / _availableUnits[i].CalculateNetProductionCost(electricityPrice);
                heatPerPrice = Math.Abs((heatPerPriceCrude < 0) ? (heatPerPriceCrude * 10) : heatPerPriceCrude);

                if(_availableUnits[i].HeatPerPriceRecords.Values.ContainsKey(hour)) // check if it needs to be reassigned or added
                    _availableUnits[i].HeatPerPriceRecords[hour] = heatPerPrice;
                else
                    _availableUnits[i].HeatPerPriceRecords.Values.Add(hour, heatPerPrice);
            }
        }
        private static void ChooseAndRecordUsage(DateTime hour, double heatDemand, double electricityPrice)
        {
            // sort units in descending order by cost efficiency (most heat per money to least)
            _availableUnits.Sort((left, right) => right.HeatPerPriceRecords[hour].CompareTo(left.HeatPerPriceRecords[hour]));
            
            double producedHeat = 0;
            double workload;
            for (int i = 0; (i < _availableUnits.Count) && (producedHeat < heatDemand); i++) 
            {
                if (_availableUnits[i].Name == MaintainedUnitName)
                {
                    if (((hour >= SummerMaintenanceStart) && (hour < SummerMaintenanceStart.AddHours(MaintenanceLength))) || ((hour >= WinterMaintenanceStart) && (hour < WinterMaintenanceStart.AddHours(MaintenanceLength))))
                    {
                        continue;
                    }
                }
                if (_availableUnits[i].MaxHeat > (heatDemand - producedHeat))
                    workload = (heatDemand - producedHeat) / _availableUnits[i].MaxHeat;
                else workload = 1;

                producedHeat += _availableUnits[i].MaxHeat * workload;

                // update records (effectively use the unit)
                _availableUnits[i].UpdateRecords(workload, _availableUnits[i].CalculateNetProductionCost(electricityPrice), hour);
            }
        }
        private static void Optimize()
        {
            // preparing for optimization
            double heatDemand;
            double electricityPrice;
            
            RetrieveUnits();
            RetrieveSourceData();
            GetSummerMaintenanceStart();
            GetWinterMaintenanceStart(); 
            
            if (TimeResolution <= 0) TimeResolution = 1;
            
            // Optimization hour by hour
            for (DateTime i = OptimizationPeriodStart; i <= OptimizationPeriodEnd; i = i.AddHours(TimeResolution))
            {
                // reset variables
                if (HeatDemand != null)
                    heatDemand = HeatDemand[i];
                else
                    throw new Exception("Heat demand not available");

                if (ElectricityPrices != null)
                    electricityPrice = ElectricityPrices[i];
                else
                    throw new Exception("Electricity prices not available");

                // Calculate heat/price ratio for this hour for every unit
                CalculateHeatPerPrice(i, electricityPrice);
                
                // Decide on which units to use, and record the usage (add values to records when used)
                ChooseAndRecordUsage(i, heatDemand, electricityPrice);
            }
        }

        public static List<IOptimizedUnit> GetResultData()
        {
            Optimize();
            return _availableUnits ?? throw new Exception("AvailableUnits not available");
        }

    }

}