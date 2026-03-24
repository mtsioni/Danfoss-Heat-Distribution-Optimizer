namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IFueledUnit
    {
        string FuelName{get; set;}
        string GetFuelName();
        void SetName(string fuelName);
        double FuelConsumption {get; set;}
        double GetFuelConsumption();
        void SetFuelConsumption(double fuelConsumption);
    }
}