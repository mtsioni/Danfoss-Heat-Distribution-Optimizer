using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Danfoss_Heat_Distribution_Optimizer.Models;

namespace Danfoss_Heat_Distribution_Optimizer.Data
{
    public static class AssetsLoader
    {
        private static string? UnitsPath { get; set; }
        private static string? GridPath { get; set; }
        private static string? LogoImagePath { get; set; }

public static void Initialize()
    {
        UnitsPath = FindAsset("ProductionUnits.json");
        GridPath = FindAsset("ProductionUnits.json");
        LogoImagePath = FindAsset("Assets/Images/DanfossLogo.png");
    }

    private static string FindAsset(string filename)
        {
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", filename);
            if (File.Exists(rootPath)) return rootPath;

            string debugPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Assets", filename);
            if (File.Exists(debugPath)) return debugPath;

            return rootPath;
        }

        /// <summary>
        /// Loads production units from JSON file specified in UnitsPath.
        /// Returns empty list on any error -> 'return new List<GenericUnits>();'
        /// </summary>
        public static List<GenericUnit> LoadUnits()
        {
            try
            {
                /// The whole process of parsing JSON
                if (string.IsNullOrEmpty(UnitsPath) || !File.Exists(UnitsPath))
                    return new List<GenericUnit>();

                string jsonContent = File.ReadAllText(UnitsPath);
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    var root = doc.RootElement;
                    if (!root.TryGetProperty("units", out var unitsArray))
                        return new List<GenericUnit>();

                    var units = new List<GenericUnit>();
                    foreach (var u in unitsArray.EnumerateArray())
                    {
                        // MAYBE DO DIFFERENT NOTATION TO NOT DECLARE VARIABLE EACH TIME
                        int unitId = u.TryGetProperty("unitID", out var id) ? id.GetInt32() : 0;
                        string name = u.TryGetProperty("name", out var n) ? n.GetString() ?? "Unknown" : "Unknown";
                        double maxHeat = u.TryGetProperty("maxHeat", out var heat) ? heat.GetDouble() : 0.0;
                        string fuelName = u.TryGetProperty("fuelName", out var fuel) ? fuel.GetString() ?? "Unknown" : "Unknown";
                        double maxElectricity = u.TryGetProperty("maxElectricity", out var elec) ? elec.GetDouble() : 0.0;
                        double productionCost = u.TryGetProperty("productionCosts", out var cost) ? cost.GetDouble() : 0.0;
                        double fuelConsumption = u.TryGetProperty("fuelConsumption", out var cons) ? cons.GetDouble() : 0.0;
                        double emissions = u.TryGetProperty("emissions", out var em) ? em.GetDouble() : 0.0;
                        string imagePath = u.TryGetProperty("imagePath", out var img) ? img.GetString() ?? "" : "";
                        var unit = new GenericUnit(unitId, name, maxHeat, fuelName, maxElectricity, productionCost, fuelConsumption, emissions, imagePath);
                        units.Add(unit);
                    }
                    return units;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading units: {ex.Message}");
                return new List<GenericUnit>();
            }
        }

        /// <summary>
        /// Loads both units and grid data from the same JSON file.
        /// Returns triple set of (units, grid, logoImagePath).
        /// </summary>
        public static (List<GenericUnit>, Grid, string) LoadGridData()
        {
            try
            {
                var units = LoadUnits();

                // Load grid from the JSON file
                Grid grid = new Grid { GridID = 1 };
                if (!string.IsNullOrEmpty(UnitsPath) && File.Exists(UnitsPath))
                {
                    string jsonContent = File.ReadAllText(UnitsPath);
                    using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("grid", out var gridData))
                        {
                            grid = new Grid
                            {
                                GridID = 1,
                                Name = gridData.TryGetProperty("nameCity", out var nameCity) ? nameCity.GetString() ?? "Unknown City" : "Unknown City",
                                Description = gridData.TryGetProperty("descriptionArchitecture", out var desc) ? desc.GetString() ?? "Unknown Architecture" : "Unknown Architecture",
                                ImagePath = gridData.TryGetProperty("imagePath", out var imgPath) ? imgPath.GetString() ?? "Assets/Images/default.png" : "Assets/Images/default.png"
                            };
                        }
                    }
                }

                return (units, grid, LogoImagePath ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading grid data: {ex.Message}");
                return (new List<GenericUnit>(), new Grid(), LogoImagePath ?? "");
            }
        }
    }
}