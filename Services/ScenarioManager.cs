using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Danfoss_Heat_Distribution_Optimizer.Models;
namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    public static class ScenarioManager
    {
        public static Scenario CurrentScenario { get; set; } = Scenario.Heat;
        private static Dictionary<Scenario, List<string>>? _scenarioJsonMapping;
        private static bool _isInitialized = false;
        private static ScenarioDTO _scenarioDTO = new();

        private static void LoadScenarios()
        {   
            if (_isInitialized)
                return;
           
            try
            {   
                // Find scenarios.json path from Asset folder
                string scenariosPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Scenarios.json");
                
                if (!File.Exists(scenariosPath))
                    scenariosPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Assets", "Scenarios.json");
                
                if (!File.Exists(scenariosPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Scenarios.json path wasn't found at: {scenariosPath}");
                    throw new Exception($"Scenario.json path wasn't found at: {scenariosPath}");
                }


                string jsonContent = File.ReadAllText(scenariosPath);
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    _scenarioJsonMapping = new();
                    var scenarios = doc.RootElement.GetProperty("scenarios");

                    foreach (var scenario in scenarios.EnumerateArray())
                    {
                        
                        var scenarioName = scenario.GetProperty("scenario").GetString();

                        if ((scenarioName != null) && Enum.TryParse<Scenario>(scenarioName, out var parsedScenario))
                        {

                            var unitNames = new List<string>();
                            var units = scenario.GetProperty("units");

                            foreach (var unit in units.EnumerateArray())
                            {
                                unitNames.Add(unit.GetString() ?? throw new Exception($"The units in json do not load. i.e. {unit.GetString()}"));
                            }

                            _scenarioJsonMapping[parsedScenario] = unitNames;
                        }
                    }
                }

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine($"ScenarioManager loaded {_scenarioJsonMapping.Count} scenarios");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading scenarios from json: {ex.Message}");
            }
        }

        public static List<GenericUnit> GetFilteredList(List<GenericUnit> inputList)
        {
            LoadScenarios();
            if (_scenarioJsonMapping == null || !_scenarioJsonMapping.ContainsKey(CurrentScenario))
                return inputList;

            _scenarioDTO.NameList = _scenarioJsonMapping[CurrentScenario];
            return _scenarioDTO.GetScenario(inputList);
        }
    }
}