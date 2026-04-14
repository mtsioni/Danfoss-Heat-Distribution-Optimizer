using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Data;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;


namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class Optimizer
    {
        public static DateTime OptimizationPeriodStart { get; set; }
        public static DateTime OptimizationPeriodEnd { get; set; }
        public static TimeSeries<double>? ElectricityPrices;
        public static TimeSeries<double>? HeatDemand;
        public static int TimeResolution { get; set; }


        private static List<IOptimizedUnit>? _availableUnits;



        private static void RetrieveUnits()
        {
            //Initilazing 
            _availableUnits = AssetManager.GetDataForOptimizer();
        }

        private static void RetriveSourceData()
        {

            ElectricityPrices = SourceDataManager.GetElectricityPrice();
            HeatDemand = SourceDataManager.GetHeatDemand();

        }

        private static double CalculateHeatPerPrice(IOptimizedUnit currentUnit, double electricityPrice)
        {
            double totalProductioncost = 1;

            if( currentUnit is CombustionUnit)
            {
                totalProductioncost = currentUnit.ProductionCost;
                
            }

            if( (currentUnit is ElectricUnit) || (currentUnit is HybridUnit))
            {
                totalProductioncost = currentUnit.ProductionCost - (electricityPrice * currentUnit.MaxHeat);
            }
            return currentUnit.MaxHeat / totalProductioncost;

        }

         
        private static void Optimize()
        {
            double heatDemand = 0;
            double producedHeat = 0;
            //List<double> heatPerPrice = new List<double>();  
            RetrieveUnits();
            RetriveSourceData();

            for (DateTime i = OptimizationPeriodStart; i <= OptimizationPeriodEnd; i = i.AddHours(TimeResolution))
            {
                if (HeatDemand != null)
                {
                    heatDemand = HeatDemand[i];
                }
                producedHeat = 0; 

                double electricityPrice = 0;

                if (_availableUnits != null)
                {
                    for (int c = 0; c < _availableUnits.Count; c++)
                    {
                        if (ElectricityPrices != null)
                        {
                            electricityPrice = ElectricityPrices[i];
                        }

                        if(_availableUnits[c].HeatPerPriceRecords.Values.ContainsKey(i))
                        {
                            _availableUnits[c].HeatPerPriceRecords[i] = CalculateHeatPerPrice(_availableUnits[c], electricityPrice);
                        }
                        else
                        {
                            _availableUnits[c].HeatPerPriceRecords.Values.Add(i, CalculateHeatPerPrice(_availableUnits[c], electricityPrice));
                        }
                        
                        
                    }

                    _availableUnits.Sort((left, right) => left.HeatPerPriceRecords[i].CompareTo(right.HeatPerPriceRecords[i]));

                    for (int c = 0;( c < _availableUnits.Count) || (producedHeat < heatDemand) ; c++) 
                    {
                        producedHeat += _availableUnits[c].MaxHeat;
                        
                        if(_availableUnits[c] is CombustionUnit)
                        {
                            
                        }
                        if(_availableUnits[c] is ElectricUnit)
                        {
                            
                        }
                        if(_availableUnits[c] is HybridUnit)
                        {
                            
                        }
                    }
                }
                /*foreach(IOptimizedUnit unit in _availableUnits ?? throw new Exception("Available Unit is not available."))
                {
                    if (ElectricityPrices != null)
                    {
                        electricityPrice = ElectricityPrices[i];
                    }
                    
                   // heatPerPrice.Insert(_availableUnits.IndexOf(unit), CalculateHeatPerPrice(unit, electricityPrice));
                }

               // heatPerPrice.Sort();
                
                foreach(double heat in heatPerPrice)
                {
                    if(producedHeat)
                }
*/
            }

            /* foreach( double heatdemand in Heatdemand)
             {

             }
            */


        }

        //public static List<IOptimizedUnit> GetOptimizationResult(){}

    }

}