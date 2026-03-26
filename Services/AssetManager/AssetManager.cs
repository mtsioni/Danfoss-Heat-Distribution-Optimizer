using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Data;
using System;

namespace Danfoss_Heat_Distribution_Optimizer.Services.AssetManager
{
    public static class AssetManager
    {
        private static List<GenericUnit> _genericUnits { get; set; } = new();
        private static Grid _grid { get; set; }
        private static string _logoImagePath { get; set; }
        private static AssetsLoader _dataLoader { get; set; }
        private static bool _isInitialized { get; set; } = false;

        // private static void LoadAssets()
        // {

        // }

        public static List<IOptimizedUnits> GetDataForOptimizer()
        {
            if(_isInitialized)
                throw new InvalidOperationException("AssetManager not initialized, call LoadAssets first.");
            return _genericUnits;
        }

        public static (List<GenericUnits>, Grid, string) GetDataForDataVisualizer()
        {
            if(_isInitialized)
                throw new InvalidOperationException("AssetManager not initialized, call LoadAssets first.");
            return (_genericUnits, _grid, _logoImagePath);
        }

    }
}