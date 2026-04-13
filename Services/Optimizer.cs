using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;


namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class Optimizer
    {
        public static DateTime OptimizationPeriodStart {get; set;}
        public static DateTime OptimizationPeriodEnd {get; set;}
        public static TimeSeries<double> ElectricityPrices;
        public static TimeSeries<double> Heatdemand; 
        public static int TimeResolution{get; set;}

        private static List<IOptimizedUnit> _availableUnits;

        private static void RetrieveUnits()
        {
            
        }
        
        private static void RetriveSourceData()
        {
            
            ElectricityPrices = SourceDataManager.GetElectricityPrice() ?? new();
            Heatdemand = SourceDataManager.GetHeatDemand() ?? new();

        }
        private static void Optimize()
        {
            
        }

        //public static List<IOptimizedUnit> GetOptimizationResult(){}

    }
   
}