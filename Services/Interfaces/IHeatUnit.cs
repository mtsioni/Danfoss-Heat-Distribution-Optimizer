namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IHeatUnit
    {
        double MaxHeat {get; set;}
        double GetMaxHeat();
        void SetMaxHeat(double maxHeat);
        double ProductionCost{get; set;}
        double GetProductionCost();
        void SetProductionCost(double productionCost);
    }
}