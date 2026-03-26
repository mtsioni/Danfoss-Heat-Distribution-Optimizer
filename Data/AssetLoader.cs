using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Danfoss_Heat_Distribution_Optimizer.Models;

namespace Danfoss_Heat_Distribution_Optimizer.Data
{
    public static class AssetsLoader
    {
        private static string _unitsPath { get; set; } = "Assets/Data/units.json";
        private static string _gridPath { get; set; } = "Assets/Data/grid.json";
        private static string _logoImagePath { get; set; } = "Assets/Images";

        public static void SetPaths(string unitsPath, string gridPath, string logoPath)
        {
            // Maybe this is too much
            _unitsPath = unitsPath;
            _gridPath = gridPath;
            _logoImagePath = logoPath;
        }
        public static List<GenericUnit> LoadUnits()
        {
            try
            {
                // TODO: Load from _unitsPath
                return new List<GenericUnit>();
            }
            catch (Exception ex)
            {
                return new List<GenericUnit>();
            }
        }

        public static (List<GenericUnit>, Grid, string) LoadGridData()
        {
            try
            {
                // TODO: Load Grid from _gridPath
                var units = LoadUnits();
                var grid = new Grid(); // TODO: Load from _gridPath
                return (units, grid, _logoImagePath);
            }
            catch (Exception ex)
            {
                return (new List<GenericUnit>(), new Grid(), _logoImagePath);
            }
        }
    }
}