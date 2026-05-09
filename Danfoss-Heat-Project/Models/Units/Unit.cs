namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public abstract class Unit
    {
        public int UnitID { get; set; } = 0;
        public string Name{ get; set; } = "";
        public double MaxHeat { get; set; } = 0;
        public double ProductionCost{ get; set; } = 0;
    }
}