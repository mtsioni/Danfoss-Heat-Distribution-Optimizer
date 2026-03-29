using System;
using CsvHelper.Configuration.Attributes;
namespace Danfoss_Heat_Distribution_Optimizer.Models
// this object is a model for how the data is represented in csv file, this is needed for CsvHelper library methods to work propperly
// every property of the object has [Index(n)] headder above, this tells CsvHelper at what position in the line of comma separated values its value will be
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