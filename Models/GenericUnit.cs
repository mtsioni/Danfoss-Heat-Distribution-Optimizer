using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
public class GenericUnit : IUnit, IElectricUnit, IFueledUnit, IHeatUnit, IGraphicalUnit
{
    // From IUnit
    public int UnitID { get; set; }
    public required string Name { get; set; }
    
    // From IElectricUnit
    public double MaxElectricity { get; set; }
    
    // From IFueledUnit  
    public required string FuelName { get; set; }
    public double FuelConsumption { get; set; }
    public double Emissions { get; set; }
    
    // From IHeatUnit
    public double MaxHeat { get; set; }
    public double ProductionCost { get; set; }

    // From IGraphicalUnit
    public string ImagePath { get; set; }

    // Constructer Generic
    public GenericUnit(int unitID, string name, double maxElectricity, string fuelName, double fuelConsumption, double emissions, double maxHeat, double productionCost, string imagePath)
        {
            UnitID = unitID;
            Name = name;
            MaxElectricity = maxElectricity;
            FuelName = fuelName;
            FuelConsumption = fuelConsumption;
            Emissions = emissions;
            MaxHeat = maxHeat;
            ProductionCost = productionCost;
            ImagePath = imagePath;
        }
}
}