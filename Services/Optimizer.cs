using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;
using Danfoss_Heat_Distribution_Optimizer.ViewModels;


namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class Optimizer
    {
        public static DateTime OptimizationPeriodStart { get; set; }
        public static DateTime OptimizationPeriodEnd { get; set; }
        public static TimeSeries<double>? ElectricityPrices { get; set; }
        public static TimeSeries<double>? HeatDemand { get; set; }
        public static int TimeResolution { get; set; } = 1;
        
        public static string MaintainedUnitName { get; set; }
        public static DateTime WinterMaintenanceStart { get; set; }
        public static DateTime SummerMaintenanceStart { get; set; }
        public static double MaintenanceLength { get; set; }
            
            
        private static List<IOptimizedUnit>? _availableUnits;


        
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
        private static void Optimize()
        {
            // preparing for optimization
            double heatDemand = 0;
            double producedHeat = 0;
            double workload = 1;
            
            //List<double> heatPerPrice = new List<double>();  
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
                {
                    heatDemand = HeatDemand[i];
                }
                producedHeat = 0; 

                double electricityPrice = 0;

                if (_availableUnits != null)
                {
                    // Calculate heat/price ratio for this hour for every unit
                    for (int c = 0; c < _availableUnits.Count; c++)
                    {
                        if (ElectricityPrices != null)
                        {
                            electricityPrice = ElectricityPrices[i];
                        }
                        else
                        {
                            throw new Exception("Electricity prices not available");
                        }
                        if(_availableUnits[c].HeatPerPriceRecords.Values.ContainsKey(i)) // check if it needs to be reassigned or added
                        {
                            _availableUnits[c].HeatPerPriceRecords[i] = _availableUnits[c].MaxHeat / _availableUnits[c].CalculateNetProductionCost(electricityPrice);
                        }
                        else
                        {
                            _availableUnits[c].HeatPerPriceRecords.Values.Add(i, _availableUnits[c].MaxHeat / _availableUnits[c].CalculateNetProductionCost(electricityPrice));
                        }
                    }

                    // sort units in ascending order by cost efficiency (most heat per money to least)
                    _availableUnits.Sort((left, right) => right.HeatPerPriceRecords[i].CompareTo(left.HeatPerPriceRecords[i]));
                    // Decide on which units to use, and record the usage (add values to records when used)
                    for (int c = 0; (c < _availableUnits.Count) && (producedHeat < heatDemand); c++) 
                    {
                        if (_availableUnits[c].Name == MaintainedUnitName)
                        {
                            if ((i >= SummerMaintenanceStart && i < SummerMaintenanceStart.AddHours(MaintenanceLength)) || (i >= WinterMaintenanceStart && i < WinterMaintenanceStart.AddHours(MaintenanceLength)))
                            {
                                continue;
                            }
                        }
                        if (_availableUnits[c].MaxHeat > (heatDemand - producedHeat))
                            workload = (heatDemand - producedHeat) / _availableUnits[c].MaxHeat;
                        else workload = 1;

                        producedHeat += _availableUnits[c].MaxHeat * workload;

                        // update records (effectively use the unit)
                        _availableUnits[c].UpdateRecords(workload, _availableUnits[c].CalculateNetProductionCost(electricityPrice), i);
                    }
                }
                else throw new Exception("AvailableUnits not available");
            }
        }

        public static List<IOptimizedUnit> GetResultData()
        {
            Optimize();
            return _availableUnits ?? throw new Exception("AvailableUnits not available");
        }

    }

}