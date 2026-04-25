using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Data;
using Danfoss_Heat_Distribution_Optimizer.Services;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;
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
        #region OPTION 1: CACHE PATTERN (In-Memory Caching) - ACTIVE BY DEFAULT
        
        private static List<GenericUnit>? _genericUnits { get; set; }
        private static Grid? _grid { get; set; }
        private static string? _logoImagePath { get; set; }
        private static bool _isInitialized { get; set; } = false;


        /// <summary>
        /// Initializes the AssetManager by discovering asset paths and loading data from JSON files.
        /// This method loads data once and keeps it in memory for fast repeated access.
        /// Can be called explicitly at startup for early error detection, but is automatically invoked on first use.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                AssetsLoader.Initialize();
                
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

        /// Gets production units for the Optimizer.
        public static List<IOptimizedUnit> GetDataForOptimizer()
        {
            // Dear team TS, 
            // I hope this comment finds you in a good state of mind
            // this is an example of Scenario implementation
            // Yours team TI
            /*
            ScenarioDTO scenario = new()
            {
                NameList = {"Electric Boiler 1", "Gas Motor 1"}
            }
            */
            if (!_isInitialized)
                Initialize();
            if (_genericUnits == null)
                throw new InvalidOperationException("AssetManager failed to load units. Check that JSON files exist and are valid.");
            GenericToOptimizedAdapter adapter = new();
            // call scenario manager GetScenarioUnits
            //return adapter.GenericToOprimizedList(scenario.GetScenario(_genericUnits));
            return adapter.GenericToOprimizedList(_genericUnits);
        }

        
        /// Gets units, grid, and logo path for the DataVisualizer.
        public static (List<GenericUnit>, Grid, string) GetDataForDataVisualizer()
        {
            if (!_isInitialized)
                Initialize();
            if (_genericUnits == null || _grid == null || _logoImagePath == null)
                throw new InvalidOperationException("AssetManager failed to load data. Check that JSON files exist and are valid.");
            
            return (_genericUnits, _grid, _logoImagePath);
        }

        #endregion

        #region OPTION 2: FACADE PATTERN (No Caching) - UNCOMMENT THIS IF WE WANT TO USE
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
    }
}