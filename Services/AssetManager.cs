using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Data;
using System;

namespace Danfoss_Heat_Distribution_Optimizer.Services
{
    /// <summary>
    /// Asset Manager serves as the central repository for system configuration data.
    /// It provides access to production units and grid information to other modules.
    /// 
    /// TEAM TODO: Decide between Option 1 (Facade; no caching in AssetManager) or Option 2 (Cache in AssetManager).
    /// - Option 1: Simpler; loads the data from JSON each time that the manager is called, so performance wise this is slower.
    /// - Option 2: Fast performance; Asset manager will be used as a 2nd cache (cause the 1st one is happening in AssetLoader) 
    ///             AssetManager will cache data in memory after initialization
    ///             aka: will load the JSON once and it will keep it in memory, so JSON won't be called everytime. 
    ///             Performance-wise this is better, cause it's a faster response.
    /// 
    /// IMPORTANT: If we choose Option 1, the UML diagram must be updated to remove
    /// _genericUnits, _grid, and _logoImagePath fields from AssetManager.
    /// </summary>
    public static class AssetManager
    {
        #region OPTION 1: FACADE PATTERN (No Caching) - UNCOMMENT THIS IF WE WANT TO USE
        /*
        private static bool _isInitialized { get; set; } = false;

        /// <summary>
        /// Initializes the AssetManager by setting paths for the data loader.
        /// Must be called before GetDataForOptimizer or GetDataForDataVisualizer.
        /// </summary>
        public static void Initialize(string unitsPath, string gridPath, string logoPath)
        {
            AssetsLoader.SetPaths(unitsPath, gridPath, logoPath);
            _isInitialized = true;
        }

        /// <summary>
        /// Gets production units for the Optimizer.
        /// Calls AssetsLoader to load data (loads each time called AKA SLOWER BUT SIMPLER).
        /// </summary>
        public static List<GenericUnits> GetDataForOptimizer()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("AssetManager not initialized. Call Initialize(unitsPath, gridPath, logoPath) first.");
            
            return AssetsLoader.LoadUnits();
        }

        /// <summary>
        /// Gets units, grid, and logo path for the DataVisualizer.
        /// Calls AssetsLoader to load data (loads each time called).
        /// </summary>
        public static (List<GenericUnits>, Grid, string) GetDataForDataVisualizer()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("AssetManager not initialized. Call Initialize(unitsPath, gridPath, logoPath) first.");
            
            return AssetsLoader.LoadGridData();
        }
        */
        #endregion

        #region OPTION 2: CACHE PATTERN (In-Memory Caching) - ACTIVE BY DEFAULT
        
        private static List<GenericUnit>? _genericUnits { get; set; }
        private static Grid? _grid { get; set; }
        private static string? _logoImagePath { get; set; }
        private static bool _isInitialized { get; set; } = false;

        /// <summary>
        /// Initializes the AssetManager by loading and caching all data from JSON files.
        /// This method loads data once and keeps it in memory for fast repeated access.
        /// Must be called before GetDataForOptimizer or GetDataForDataVisualizer.
        /// </summary>
        public static void Initialize(string unitsPath, string gridPath, string logoPath)
        {
            try
            {
                AssetsLoader.SetPaths(unitsPath, gridPath, logoPath);
                
                // Load all data once and cache it in memory AKA FASTER
                var (units, grid, logo) = AssetsLoader.LoadGridData();
                
                _genericUnits = units;
                _grid = grid;
                _logoImagePath = logo;
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize AssetManager. Check that JSON files exist and are valid.", ex);
            }
        }

        /// <summary>
        /// Gets production units for the Optimizer.
        /// Returns cached data (loaded during Initialize).
        /// </summary>
        public static List<GenericUnit> GetDataForOptimizer()
        {
            if (!_isInitialized || _genericUnits == null)
                throw new InvalidOperationException("AssetManager not initialized. Call Initialize(unitsPath, gridPath, logoPath) first.");
            
            return _genericUnits;
        }

        /// <summary>
        /// Gets units, grid, and logo path for the DataVisualizer.
        /// Returns cached data (loaded during Initialize).
        /// </summary>
        public static (List<GenericUnit>, Grid, string) GetDataForDataVisualizer()
        {
            if (!_isInitialized || _genericUnits == null || _grid == null || _logoImagePath == null)
                throw new InvalidOperationException("AssetManager not initialized. Call Initialize(unitsPath, gridPath, logoPath) first.");
            
            return (_genericUnits, _grid, _logoImagePath);
        }

        #endregion
    }
}


        #region HOW TO USE IT

        /// ... in Main or startup method eg.

        //AssetManager.Initialize(
        //     unitsPath: "Assets/ProductionUnits.json",
        //     gridPath: "Assets/ProductionUnits.json", 
        //     logoPath: "Assets/Images/DanfossLogo.png"
        // );


        /// ... anywhere else ...
        
        // 1. Get data for the Optimizer module

        // var units = AssetManager.GetDataForOptimizer();
        // foreach (var unit in units)
        // {
        //     Console.WriteLine($"Unit: {unit.Name}, Max Heat: {unit.MaxHeat} MW");
        // }
        
        // 2. Get data for the Visualizer module

        // var (allUnits, gridInfo, logoPath) = AssetManager.GetDataForDataVisualizer();
        // Console.WriteLine($"Grid: {gridInfo.Name}");
        // Console.WriteLine($"Logo: {logoPath}");


        #endregion