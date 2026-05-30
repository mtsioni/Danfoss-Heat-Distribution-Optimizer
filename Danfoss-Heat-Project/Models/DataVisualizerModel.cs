using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Services;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class DataVisualizerModel
    {
        public TimeSeries<double> ProductionCostRecords { get; set; } = new();

        public TimeSeries<double> ElectricityProduced { get; set; } = new();
        public TimeSeries<double> ElectricityConsumed { get; set; } = new();
        public TimeSeries<double> MoneyEarned { get; set; } = new();
        public TimeSeries<double> MoneySpent { get; set; } = new();
        public TimeSeries<double> Co2Emissions { get; set; } = new();
        public TimeSeries<double> FuelConsumption { get; set; } = new();
        public TimeSeries<double> HeatProduced { get; set; } = new();
        
        public void UpdateData()
        {
            var units = ResultDataManager.GetResultData();
            
            ClearAll();

            if (units == null || units.Count == 0) return;

            foreach (var unit in units)
            {
                AggregateByCondition(ProductionCostRecords, unit.ProductionCostRecords, v => true);

                AggregateByCondition(HeatProduced, unit.HeatRecords, v => true);
                AggregateByCondition(ElectricityProduced, unit.ElectricityRecords, v => v >= 0);
                AggregateByCondition(ElectricityConsumed, unit.ElectricityRecords, v => v <= 0, v => Math.Abs(v));
                AggregateByCondition(MoneySpent, unit.ProductionCostRecords, v => v >= 0);
                AggregateByCondition(MoneyEarned, unit.ProductionCostRecords, v => v <= 0, v => Math.Abs(v));
                AggregateByCondition(Co2Emissions, unit.PollutionRecords, v => true);
                AggregateByCondition(FuelConsumption, unit.FuelConsumptionRecords, v => true);
            }
        }

        private void ClearAll()
        {
            ProductionCostRecords.Values.Clear();

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
