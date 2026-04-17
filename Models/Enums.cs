
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
        HeatProduced,
        HeatConsumed,
        ElectricityProduced,
        ElectricityConsumed,
        ElectricityPrice,
        MoneyEarned,
        MoneySpent,
        Co2Emissions,
        FuelConsumption,
        Profit,
        Expenses
    }
}
