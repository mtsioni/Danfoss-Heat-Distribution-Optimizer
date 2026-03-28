using Avalonia.Controls;
using ReactiveUI;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels;

public class UnitViewModel : ReactiveObject
{
    public string Name {get;}
    public double MaxHeat {get;}
    public string MaxHeatDisplayText {get => MaxHeat > 0 ? $"MaxHeat: {MaxHeat} MWh" : string.Empty;}
    public double? MaxElectricity {get;}
    public string MaxElectricityDisplayText {get => MaxElectricity < 0 ? $"Elec: {MaxElectricity} MWh" : string.Empty;}
    public double Cost {get;}
    public string CostDisplayText {get => Cost > 0 ? $"Cost: {Cost} DKK" : string.Empty;}
    public double? CO2 {get;}
    public string Co2DisplayText {get => CO2 > 0 ? $"CO2: {CO2} kg" : string.Empty;} 
    public double? FuelConsumption {get;}
    public string FuelConsumptionDisplayText {get => FuelConsumption > 0 ? $"Fuel: {FuelConsumption} MWh" : string.Empty;}
    public string ImagePath {get;}

    public UnitViewModel(string name, double maxHeat, double? maxElectricity, double cost, double? co2, double? fuelConsumption, string imagePath)
    {
        Name = name;
        MaxHeat = maxHeat;
        MaxElectricity = maxElectricity;
        Cost = cost;
        CO2 = co2; 
        FuelConsumption = fuelConsumption;
        ImagePath = imagePath;   
    }
}