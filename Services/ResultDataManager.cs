using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Data;
using Danfoss_Heat_Distribution_Optimizer.ViewModels;
using System;
using System.Linq;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class ResultDataManager
    {
        public static DateTime OptimizationPeriodStart { get; set; }
        public static DateTime OptimizationPeriodEnd { get; set; }
        public static int TimeResolution { get; set; }
        private static List<IOptimizedUnit>? _resultData { get; set; }
        
        private static void ProcessResults()
        {
            _resultData = Optimizer.GetResultData();
            SaveResultData();
        }

        private static void SaveResultData()
        {
            if (_resultData == null || _resultData.Count == 0)
                return;
            
            foreach (var unit in _resultData)
            {
                List<ResultDataDTO> hourlyRecords = new List<ResultDataDTO>();

                // Calculate total hours from optimization period
                int totalHours = (int)(OptimizationPeriodEnd - OptimizationPeriodStart).TotalHours;

                // Extract hourly data from TimeSeries properties
                for (int i = 0; i <= totalHours; i++)
                {
                    DateTime currentTime = OptimizationPeriodStart.AddHours(i * TimeResolution);

                    var dto = new ResultDataDTO
                    {
                        UnitName = unit.Name,
                        Time = currentTime,
                        ProductionCost = unit.ProductionCostRecords[currentTime],
                        HeatMWh = unit.HeatRecords[currentTime],
                        Electricity = unit.ElectricityRecords[currentTime],
                        Co2Emissions = unit.PollutionRecords[currentTime],
                        FuelConsumption = unit.FuelConsumptionRecords[currentTime]
                    };

                    hourlyRecords.Add(dto);
                }

                /// Check if the code write into the CVS two times - might be wrong and we need to delete it
                // var groups = hourlyRecords.GroupBy(x => x.UnitName);

                // foreach (var group in groups)
                // {
                //     string name = group.Key ?? "Unknown";
                //     ResultDataSaver.SaveToCSV(name, group.ToList());
                //     Console.WriteLine($"Saved {name}.csv");
                // }

                ResultDataSaver.SaveToCSV(unit.Name, hourlyRecords);
            }
        }

        // called by DataVisualizer
        public static List<IOptimizedUnit> GetResultData()
        {
            ProcessResults();
            
            if (_resultData == null)
                return new List<IOptimizedUnit>();

            return _resultData;
        }
    }
}