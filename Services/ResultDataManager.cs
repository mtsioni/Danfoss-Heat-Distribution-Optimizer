using System;
using System.Collections.Generic;
using System.Linq;
using Danfoss_Heat_Distribution_Optimizer.Models;

namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public class ResultDataManager
    {
        public TimeSeries<double> GetTimeSeriesData(DataKind kind, Period period)
        {
            // Use the specific dates found in SourceData.csv
            DateTime referenceDate = (period == Period.Winter) 
                ? new DateTime(2026, 1, 5) 
                : new DateTime(2025, 9, 8);

            if (kind == DataKind.HeatConsumed)
            {
                var realData = SourceDataManager.GetHeatDemand();
                if (realData != null) return FilterByDate(realData, referenceDate);
            }

            if (kind == DataKind.ElectricityPrice)
            {
                var realData = SourceDataManager.GetElectricityPrice();
                if (realData != null) return FilterByDate(realData, referenceDate);
            }

            var ts = new TimeSeries<double>();
            for (int i = 0; i < 24; i++)
            {
                DateTime time = referenceDate.AddHours(i);
                double value = 0;
                switch (kind)
                {
                    case DataKind.HeatProduced:
                        value = 0 + i * 0.2;
                        break;
                    case DataKind.HeatConsumed:
                        value = 150 + Math.Sin(i * 0.5) * 30;
                        break;
                    case DataKind.ElectricityProduced:
                        value = 60 + i * 2;
                        break;
                    case DataKind.ElectricityConsumed:
                        value = 80 + i * 1.5;
                        break;
                    case DataKind.MoneyEarned:
                    case DataKind.Profit:
                        value = 200 + i * 10;
                        break;
                    case DataKind.MoneySpent:
                    case DataKind.Expenses:
                        value = 150 + i * 8;
                        break;
                    case DataKind.Co2Emissions:
                        value = 30 + Math.Cos(i * 0.3) * 5;
                        break;
                    case DataKind.FuelConsumption:
                    case DataKind.PrimaryEnergy:
                        value = 40 + i * 2;
                        break;
                }
                ts.Values[time] = value;
            }

            return ts;
        }

        private TimeSeries<double> FilterByDate(TimeSeries<double> source, DateTime date)
        {
            var filtered = new TimeSeries<double>();
            foreach (var kvp in source.Values)
            {
                // Only take data for the specified day
                if (kvp.Key.Date == date.Date)
                {
                    filtered.Values[kvp.Key] = kvp.Value;
                }
            }
            return filtered;
        }
    }
}