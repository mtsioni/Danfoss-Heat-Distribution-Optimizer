using System.Collections.Generic;
using System.Net;
using Danfoss_Heat_Distribution_Optimizer.Models;
namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class ScenarioManager
    {
        public static Scenario Scenario { get; set; } = Scenario.Heat;
        public static List<GenericUnit> GetFilteredList(List<GenericUnit> inputList)
        {
            inputList = new();
            return inputList;
        }
    }
}