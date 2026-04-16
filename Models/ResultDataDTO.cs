using System;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class ResultDataDTO
    {
        public string? UnitName {get;set;}
        public DateTime Time {get;set;}
        public double HeatMWh {get;set;}
        public double Electricity {get;set;}
        public double ProductionCost {get;set;}
        public double PrimaryEnergy {get;set;}
        public double Co2Emissions {get;set;}
        public double NetCost {get;set;}
    }
}