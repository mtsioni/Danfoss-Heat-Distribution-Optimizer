using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels;

public class UnitViewModel : ReactiveObject
{
    public string Name { get; }
    public string FuelName { get; }
    public double MaxHeat { get; }
    public double MaxElectricity { get; }
    public double Cost { get; }
    public double CO2 { get; }
    public double FuelConsumption { get; }
    public Bitmap? Image { get; }

    public bool HasElectricity => MaxElectricity != 0;

    // Bullet-formatted stat lines shown in the card
    public string MaxHeatText => $"  •  {MaxHeat} MW";
    public string MaxElectricityText => $"  •  {MaxElectricity} MW";
    public string ProductionCostText => $"  •  {Cost} DKK/MWh(th)";
    public string Co2Text => $"  •  {CO2} kg/MWh(th)";

    public string FuelConsumptionLabel
    {
        get => FuelName switch
        {
            "Gas"      => "Gas Consumption",
            "Oil"      => "Oil Consumption",
            "Electric" => "Electricity Consumption",
            _          => $"{FuelName} Consumption"
        };
    }

    public string FuelConsumptionText
    {
        get
        {
            string unit = FuelName switch
            {
                "Gas"      => "gas",
                "Oil"      => "oil",
                "Electric" => "elec",
                _          => FuelName.ToLower()
            };
            return $"  •  {FuelConsumption} MWh({unit})/MWh(th)";
        }
    }

    public bool HasFuelConsumption => FuelConsumption > 0;

    public UnitViewModel(string name, string fuelName, double maxHeat, double maxElectricity, double cost, double co2, double fuelConsumption, string imagePath)
    {
        Name = name;
        FuelName = fuelName;
        MaxHeat = maxHeat;
        MaxElectricity = maxElectricity;
        Cost = cost;
        CO2 = co2;
        FuelConsumption = fuelConsumption;

        try
        {
            Image = new Bitmap(AssetLoader.Open(new Uri(imagePath)));
        }
        catch
        {
            Image = null;
        }
    }
}