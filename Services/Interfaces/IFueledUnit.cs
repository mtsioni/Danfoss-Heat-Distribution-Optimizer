namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IFueledUnit
    {
        string FuelName{ get; set; }
        double FuelConsumption { get; set; }
        double Emissions { get; set; }
    }
}