
using CsvHelper.Configuration;
// this is a map for the values from SourceDataModel, which works as a config for CsvHelper's file reader
// this map shows at which position in a line of comma separated values are values for specific properties
// this does the same thing as headders in SourceDataModel, but for some reason if any headder or map is removed it doesn't work that's why it's done in this redundant way
namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public sealed class SourceDataModelHeatDemandMap : ClassMap<SourceDataModelHeatDemand>
    {
        public SourceDataModelHeatDemandMap()
        {
            Map(m => m.WinterTimeStamp).Index(0);

            Map(m => m.WinterValue).Index(2);

            Map(m => m.SummerTimeStamp).Index(5);

            Map(m => m.SummerValue).Index(7);
        }
    }
    public sealed class SourceDataModelElectricityPriceMap : ClassMap<SourceDataModelHeatDemand>
    {
        public SourceDataModelElectricityPriceMap()
        {
            Map(m => m.WinterTimeStamp).Index(0);

            Map(m => m.WinterValue).Index(3);

            Map(m => m.SummerTimeStamp).Index(5);

            Map(m => m.SummerValue).Index(8);
        }
    }
}