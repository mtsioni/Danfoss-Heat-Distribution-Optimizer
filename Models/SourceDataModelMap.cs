
using CsvHelper.Configuration;

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