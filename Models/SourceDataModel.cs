using System;
using CsvHelper.Configuration.Attributes;
namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class SourceDataModelHeatDemand
    {
        [Index(0)]
        public DateTime WinterTimeStamp { get; set; }
        [Index(2)]
        public double WinterValue { get; set; }
        [Index(5)]
        public DateTime SummerTimeStamp { get; set; }
        [Index(7)]
        public double SummerValue { get; set; }

    }
    public class SourceDataModelElectricityPrice
    {
        [Index(0)]
        public DateTime WinterTimeStamp { get; set; }
        [Index(3)]
        public double WinterValue { get; set; }
        [Index(5)]
        public DateTime SummerTimeStamp { get; set; }
        [Index(8)]
        public double SummerValue { get; set; }

    }
}