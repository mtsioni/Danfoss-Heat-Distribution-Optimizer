namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IHeatUnit
    {
        double MaxHeat { get; set; }
        double ProductionCost{ get; set; }
    }
}