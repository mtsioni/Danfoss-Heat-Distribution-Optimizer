namespace Danfoss_Heat_Distribution_Optimizer.Services.Interfaces
{
    public interface IElectricUnit
    {
        double MaxElectricity {get; set;}
        double GetMaxElectricity();
        void SetMaxElectricity(double maxElectricity);
    }
}