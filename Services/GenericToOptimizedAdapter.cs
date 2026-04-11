using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public class GenericToOptimizedAdapter()
    {
        public IOptimizedUnit GenericToOptimized(GenericUnit inputUnit)
        {
            bool electric = false;
            bool fueled = false;


            if (!string.IsNullOrEmpty(inputUnit.FuelName))
            {
                fueled = true;
            }

            if (inputUnit.MaxElectricity != null)
            {
                electric = true;
            }

            if ((electric == true) && (fueled == true))
            {
                HybridUnit outputUnit = new HybridUnit(inputUnit.UnitID, inputUnit.Name, inputUnit.MaxElectricity, inputUnit.FuelName,
                                                       inputUnit.FuelConsumption, inputUnit.Emissions, inputUnit.MaxHeat, inputUnit.ProductionCost);

                return outputUnit;
            }

            if ((electric == true) && (fueled != true))
            {
                ElectricUnit outputUnit = new ElectricUnit(inputUnit.UnitID, inputUnit.Name, inputUnit.MaxElectricity,
                                                           inputUnit.MaxHeat, inputUnit.ProductionCost);

                return outputUnit;
            }
           
            if ((electric != true) && (fueled == true))
            {
                CombustionUnit outputUnit = new CombustionUnit(inputUnit.UnitID, inputUnit.Name, inputUnit.FuelName,
                                                               inputUnit.FuelConsumption, inputUnit.Emissions, inputUnit.MaxHeat, inputUnit.ProductionCost);

                return outputUnit;
            }

            throw new Exception ("Invalid type");
        }
    }
}