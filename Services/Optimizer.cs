using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;


namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class Optimizer
    {
        public static DateTime OptimizationPeriodStart { get; set; }
        public static DateTime OptimizationPeriodEnd { get; set; }
        public static TimeSeries<double>? ElectricityPrices { get; set; }
        public static TimeSeries<double>? HeatDemand { get; set; }
        public static int TimeResolution { get; set; }


        private static List<IOptimizedUnit>? _availableUnits;



        private static void RetrieveUnits()
        {
            // May need to initialize
            _availableUnits = AssetManager.GetDataForOptimizer();
        }

        private static void RetriveSourceData()
        {

            ElectricityPrices = SourceDataManager.GetElectricityPrice();
            HeatDemand = SourceDataManager.GetHeatDemand();

        }
        private static void Optimize()
        {
            // preparing for optimization
            double heatDemand = 0;
            double producedHeat = 0;
            //List<double> heatPerPrice = new List<double>();  
            RetrieveUnits();
            RetriveSourceData();
            
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
                            _availableUnits[c].HeatPerPriceRecords[i] = _availableUnits[c].MaxHeat / _availableUnits[c].CalculateTotalProductionCost(electricityPrice);
                        }
                        else
                        {
                            _availableUnits[c].HeatPerPriceRecords.Values.Add(i, _availableUnits[c].MaxHeat / _availableUnits[c].CalculateTotalProductionCost(electricityPrice));
                        }
                    }

                    // sort units in ascending order by cost efficiency (most heat per money to least)
                    _availableUnits.Sort((left, right) => left.HeatPerPriceRecords[i].CompareTo(right.HeatPerPriceRecords[i])); 

                    // Decide on which units to use, and record the usage (add values to records when used)
                    for (int c = 0;( c < _availableUnits.Count) || (producedHeat < heatDemand) ; c++) 
                    {
                        // increase producedHeat to try reach demand
                        producedHeat += _availableUnits[c].MaxHeat;
                        // update records (effectively use the unit)
                        _availableUnits[c].UpdateRecords(electricityPrice, i);
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