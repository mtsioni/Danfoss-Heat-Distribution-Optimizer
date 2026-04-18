using System;
using System.Collections.Generic;
using System.Linq;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;
using Danfoss_Heat_Distribution_Optimizer.Services;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class DataVisualizerModel
    {
        // Raw Aggregated TimeSeries properties corresponding to IOptimizedUnit fields
        public TimeSeries<double> ProductionCostRecords { get; set; } = new();
        public TimeSeries<double> HeatRecords { get; set; } = new();
        public TimeSeries<double> ElectricityRecords { get; set; } = new();
        public TimeSeries<double> PollutionRecords { get; set; } = new();
        public TimeSeries<double> FuelConsumptionRecords { get; set; } = new();

        // Specific aggregated properties for UI convenience
        public TimeSeries<double> ElectricityProduced { get; set; } = new();
        public TimeSeries<double> ElectricityConsumed { get; set; } = new();
        public TimeSeries<double> MoneyEarned { get; set; } = new();
        public TimeSeries<double> MoneySpent { get; set; } = new();
        public TimeSeries<double> Co2Emissions { get; set; } = new();
        public TimeSeries<double> FuelConsumption { get; set; } = new();
        public TimeSeries<double> HeatProduced { get; set; } = new();

        // HeatPerPriceRecords excluded from aggregation

        /// <summary>
        /// Refreshes the model by fetching latest result data and aggregating it.
        /// </summary>
        public void UpdateData()
        {
            var units = ResultDataManager.GetResultData();
            
            // Clear existing aggregated data
            ClearAll();

            if (units == null || units.Count == 0) return;

            foreach (var unit in units)
            {
                // Basic aggregation
                AggregateByCondition(ProductionCostRecords, unit.ProductionCostRecords, v => true);
                AggregateByCondition(HeatRecords, unit.HeatRecords, v => true);
                AggregateByCondition(ElectricityRecords, unit.ElectricityRecords, v => true);
                AggregateByCondition(PollutionRecords, unit.PollutionRecords, v => true);
                AggregateByCondition(FuelConsumptionRecords, unit.FuelConsumptionRecords, v => true);

                // Refined aggregation based on value signs (logic from ViewModel)
                AggregateByCondition(HeatProduced, unit.HeatRecords, v => true); // All heat is produced
                AggregateByCondition(ElectricityProduced, unit.ElectricityRecords, v => v > 0);
                AggregateByCondition(ElectricityConsumed, unit.ElectricityRecords, v => v < 0, v => Math.Abs(v));
                AggregateByCondition(MoneySpent, unit.ProductionCostRecords, v => v > 0);
                AggregateByCondition(MoneyEarned, unit.ProductionCostRecords, v => v < 0, v => Math.Abs(v));
                AggregateByCondition(Co2Emissions, unit.PollutionRecords, v => true);
                AggregateByCondition(FuelConsumption, unit.FuelConsumptionRecords, v => true);
            }
        }

        private void ClearAll()
        {
            ProductionCostRecords.Values.Clear();
            HeatRecords.Values.Clear();
            ElectricityRecords.Values.Clear();
            PollutionRecords.Values.Clear();
            FuelConsumptionRecords.Values.Clear();

            ElectricityProduced.Values.Clear();
            ElectricityConsumed.Values.Clear();
            MoneyEarned.Values.Clear();
            MoneySpent.Values.Clear();
            Co2Emissions.Values.Clear();
            FuelConsumption.Values.Clear();
            HeatProduced.Values.Clear();
        }

        private void AggregateByCondition(TimeSeries<double> target, TimeSeries<double> source, Func<double, bool> condition, Func<double, double>? transform = null)
        {
            if (source == null || source.Values == null) return;
            foreach (var kvp in source.Values)
            {
                if (condition(kvp.Value))
                {
                    double finalVal = transform != null ? transform(kvp.Value) : kvp.Value;
                    if (target.Values.ContainsKey(kvp.Key))
                        target.Values[kvp.Key] += finalVal;
                    else
                        target.Values[kvp.Key] = finalVal;
                }
            }
        }
    }
}
