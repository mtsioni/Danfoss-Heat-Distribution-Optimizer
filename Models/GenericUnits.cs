using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
public class GenericUnits : IUnit, IElectricUnit, IFueledUnit, IHeatUnit
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
}
}