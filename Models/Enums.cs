
namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public enum Scenario
    {
        Heat,
        HeatAndElectricity
    }

    public enum Period
    {
        Winter,
        Summer
    }

    public enum DataKind
    {
        HeatDemand,
        HeatProduction,
        Electricity,
        ElectricityPrice,
        Co2Emissions,
        FuelConsumption,
        ProductionCosts
    }
}
