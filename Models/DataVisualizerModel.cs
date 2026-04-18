using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class DataVisualizerModel
    {
        public List<IOptimizedUnit> Units { get; set; }

        public DataVisualizerModel(List<IOptimizedUnit> units)
        {
            Units = units;
        }

        public void PrintData()
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("DataVisualizerModel: START");
            Console.WriteLine("==========================================================");

            if (Units == null || Units.Count == 0)
            {
                Console.WriteLine("NO UNITS FOUND");
            }
            else
            {
                foreach (var unit in Units)
                {
                    Console.WriteLine($"UNIT NAME: {unit.Name}");
                    PrintSeries("Heat Produced (MWh)", unit.HeatRecords);
                    PrintSeries("Production Cost (DKK)", unit.ProductionCostRecords);
                    PrintSeries("Electricity (MWh)", unit.ElectricityRecords);
                    PrintSeries("Pollution (kg CO2)", unit.PollutionRecords);
                    PrintSeries("Fuel Consumption (MWh)", unit.FuelConsumptionRecords);
                    PrintSeries("Heat Per Price", unit.HeatPerPriceRecords);
                    Console.WriteLine("----------------------------------------------------------");
                }
            }

            Console.WriteLine("==========================================================");
            Console.WriteLine("END");
            Console.WriteLine("==========================================================");
        }

        private void PrintSeries(string name, TimeSeries<double> series)
        {
            if (series == null || series.Values == null)
            {
                Console.WriteLine($"  [{name}] - NULL");
                return;
            }

            Console.WriteLine($"  [{name}] - {series.Values.Count} entries:");
            foreach (var kvp in series.Values)
            {
                Console.WriteLine($"    {kvp.Key:yyyy-MM-dd HH:mm} => {kvp.Value,7:F4}");
            }
        }
    }
}
