namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class GenericUnit : Unit
    {
        // ElectricUnit
        public double MaxElectricity { get; set; }
    
        // FueledUnit  
        public string FuelName { get; set; }
        public double FuelConsumption { get; set; }
        public double Emissions { get; set; }

        // GraphicalUnit
        public string ImagePath { get; set; }


        // Constructer Generic
        public GenericUnit(int unitID, string name,
                        double maxElectricity, 
                        string fuelName, double fuelConsumption, double emissions, 
                        double maxHeat, double productionCost, 
                        string imagePath)
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
        public GenericUnit()
        {
            UnitID = 0;
            Name = "";
            MaxElectricity = 0.0;
            FuelName = "";
            FuelConsumption = 0.0;
            Emissions = 0.0;
            MaxHeat = 0.0;
            ProductionCost = 0.0;
            ImagePath = "";
        }
    }
}