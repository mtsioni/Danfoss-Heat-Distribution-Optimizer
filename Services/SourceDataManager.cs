using System;
using Danfoss_Heat_Distribution_Optimizer.Data;
using Danfoss_Heat_Distribution_Optimizer.Models;


namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    
    public static class SourceDataManager
    {
        public static DateTime OptimizationPeriodStart
        {
            get;
            set;
        }
        public static DateTime OptimizationPeriodEnd 
        {
            get;
            set;
        }

        public static int TimeResolution {get; set;}

        private static TimeSeries<double>? _heatDemand;
        private static TimeSeries<double>? _electricityPrices;
        
        private static void LoadSourceData()
        {
            _heatDemand = SourceDataLoader.LoadHeatDemand();
            _electricityPrices = SourceDataLoader.LoadElectricityPrices();

        }
        
        public static TimeSeries<double>? GetHeatDemand()
        {
            if (_heatDemand == null)
            {
                LoadSourceData();
            }
            return _heatDemand;
        }

        public static TimeSeries<double>? GetElectricityPrice()
        {
            if (_electricityPrices == null)
            {
                LoadSourceData();
            }
            return _electricityPrices;
        }

    }

}