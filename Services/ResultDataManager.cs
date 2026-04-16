using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Data;
using System;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class ResultDataManager
    {
        public static DateTime OptimizationPeriodStart { get; set; }
        public static DateTime OptimizationPeriodEnd { get; set; }
        public static int TimeResolution { get; set; }
        private static List<IOptimizedUnit>? _resultData { get; set; }
        private static ResultDataSaver? _dataSaver { get; set; }

        // called by Optimizer
        public static void ProcessResults(List<IOptimizedUnit> results)
        {
            _resultData = results;
            SaveResultData();
        }

        private static void SaveResultData()
        {
            if (_resultData == null || _resultData.Count == 0)
                return;
            
            //Need to do DTO shenanigans for the FUEL INFO

            foreach (var unit in _resultData)
            {
                List<ResultDataDTO> hourlyRecords = new List<ResultDataDTO>();

                // Calculate total hours from optimization period
                int totalHours = (int)(OptimizationPeriodEnd - OptimizationPeriodStart).TotalHours;

                // Extract hourly data from TimeSeries properties
                for (int i = 0; i < totalHours; i++)
                {
                    DateTime currentTime = OptimizationPeriodStart.AddMinutes(i * TimeResolution);

                    var dto = new ResultDataDTO
                    {
                        UnitName = unit.Name,
                        Time = currentTime,
                        HeatMWh = unit.HeatRecords[currentTime],
                        Electricity = unit.ElectricityRecords[currentTime],
                        ProductionCost = unit.ProductionCostRecords[currentTime],
                        Co2Emissions = unit.PollutionRecords[currentTime],
                        NetCost = unit.NetProductionCost  // Total net cost for this unit
                    };

                    hourlyRecords.Add(dto);
                }

                ResultDataSaver.SaveToCSV(unit.Name, hourlyRecords);
            }
        }

        // called by DataVisualizer
        public static List<IOptimizedUnit> GetResultData()
        {
            if (_resultData == null)
                return new List<IOptimizedUnit>();

            return _resultData;
        }
    }
}