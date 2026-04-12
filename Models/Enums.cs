
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

    public enum ChartOption
    {
        HeatProduced,
        HeatConsumed,
        ElectricityProduced,
        ElectricityConsumed,
        MoneyEarned,
        MoneySpent
    }
}
