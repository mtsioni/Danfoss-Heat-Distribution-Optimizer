using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using ReactiveUI;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services;
using OxyPlot;
using System.Xml.Serialization;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    // visualizer holds all the graph logic so we dont clutter this file too much
    public DataVisualizerViewModel Visualizer { get; } = new();

    // =# SCENARIO STUFF #=
    // keeps track of what scenario the user choosed
    // we use a private variable with a public getter/setter so we can inject RaiseAndSetIfChanged
    private Scenario _selectedScenario = Scenario.Heat;
    public Scenario SelectedScenario
    {
        get => _selectedScenario;
        set => this.RaiseAndSetIfChanged(ref _selectedScenario, value);
    }

    public bool IsHeatScenarioSelected
    {
        get => SelectedScenario == Scenario.Heat;
        set
        {
            if (value) 
            {
                SelectedScenario = Scenario.Heat;
                ShowElectricity = false;
                ShowElectricityPrice = false;
                RefreshDataAndChart();
            }
            this.RaisePropertyChanged(nameof(IsHeatScenarioSelected));
            this.RaisePropertyChanged(nameof(IsHeatAndElecScenarioSelected));
        }
    }

    public bool IsHeatAndElecScenarioSelected
    {
        get => SelectedScenario == Scenario.HeatAndElectricity;
        set
        {
            if (value) 
            {
                SelectedScenario = Scenario.HeatAndElectricity;
                RefreshDataAndChart();
            }
            this.RaisePropertyChanged(nameof(IsHeatScenarioSelected));
            this.RaisePropertyChanged(nameof(IsHeatAndElecScenarioSelected));
        }
    }

    private Period _selectedPeriod = Period.Winter;
    public Period SelectedPeriod
    {
        get => _selectedPeriod;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPeriod, value);
            RefreshDataAndChart();
            this.RaisePropertyChanged(nameof(IsWinterSelected));
            this.RaisePropertyChanged(nameof(IsSummerSelected));
        }
    }

    private void SyncOptimizationPeriod()
    {
        if (SelectedPeriod == Period.Winter)
        {
            Optimizer.OptimizationPeriodStart = new DateTime(2026, 1, 5);
            Optimizer.OptimizationPeriodEnd = new DateTime(2026, 1, 19);
        }
        else
        {
            Optimizer.OptimizationPeriodStart = new DateTime(2025, 9, 8);
            Optimizer.OptimizationPeriodEnd = new DateTime(2025, 9, 22);
        }

        Optimizer.TimeResolution = 1;

        // Sync with ResultDataManager
        ResultDataManager.OptimizationPeriodStart = Optimizer.OptimizationPeriodStart;
        ResultDataManager.OptimizationPeriodEnd = Optimizer.OptimizationPeriodEnd;
        ResultDataManager.TimeResolution = Optimizer.TimeResolution;
    }

    public bool IsWinterSelected { get => SelectedPeriod == Period.Winter; set { if (value) SelectedPeriod = Period.Winter; RefreshDataAndChart();} }
    public bool IsSummerSelected { get => SelectedPeriod == Period.Summer; set { if (value) SelectedPeriod = Period.Summer; RefreshDataAndChart(); } }

    private double _maintenanceHours = 30;
    public double MaintenanceHours { get => _maintenanceHours; set { this.RaiseAndSetIfChanged(ref _maintenanceHours, value); RefreshDataAndChart(); } }

    private UnitViewModel? _selectedUnit;
    public UnitViewModel? SelectedUnit { get => _selectedUnit; set { this.RaiseAndSetIfChanged(ref _selectedUnit, value); RefreshDataAndChart(); } }

    private bool _showHeatDemand = true;
    public bool ShowHeatDemand { get => _showHeatDemand; set { this.RaiseAndSetIfChanged(ref _showHeatDemand, value); UpdateChart(); } }

    private bool _showHeatProduction = true;
    public bool ShowHeatProduction { get => _showHeatProduction; set { this.RaiseAndSetIfChanged(ref _showHeatProduction, value); UpdateChart(); } }

    private bool _showElectricity;
    public bool ShowElectricity { get => _showElectricity; set { this.RaiseAndSetIfChanged(ref _showElectricity, value); UpdateChart(); } }

    private bool _showElectricityPrice;
    public bool ShowElectricityPrice { get => _showElectricityPrice; set { this.RaiseAndSetIfChanged(ref _showElectricityPrice, value); UpdateChart(); } }

    private bool _showCo2Emissions;
    public bool ShowCo2Emissions { get => _showCo2Emissions; set { this.RaiseAndSetIfChanged(ref _showCo2Emissions, value); UpdateChart(); } }

    private bool _showFuelConsumption;
    public bool ShowFuelConsumption { get => _showFuelConsumption; set { this.RaiseAndSetIfChanged(ref _showFuelConsumption, value); UpdateChart(); } }

    private bool _showProductionCosts;
    public bool ShowProductionCosts { get => _showProductionCosts; set { this.RaiseAndSetIfChanged(ref _showProductionCosts, value); UpdateChart(); } }

    private bool _isEnergyEnabled = true;
    public bool IsEnergyEnabled { get => _isEnergyEnabled; set => this.RaiseAndSetIfChanged(ref _isEnergyEnabled, value); }
    
    private bool _isFinanceEnabled = true;
    public bool IsFinanceEnabled { get => _isFinanceEnabled; set => this.RaiseAndSetIfChanged(ref _isFinanceEnabled, value); }

    private bool _isCo2Enabled = true;
    public bool IsCo2Enabled { get => _isCo2Enabled; set => this.RaiseAndSetIfChanged(ref _isCo2Enabled, value); }
    
    private bool _isFuelEnabled = true;
    public bool IsFuelEnabled { get => _isFuelEnabled; set => this.RaiseAndSetIfChanged(ref _isFuelEnabled, value); }

    private bool _isElectricityEnabled = true;
    public bool IsElectricityEnabled { get => _isElectricityEnabled; set => this.RaiseAndSetIfChanged(ref _isElectricityEnabled, value); }

    private bool _isElectricityPriceEnabled = true;
    public bool IsElectricityPriceEnabled { get => _isElectricityPriceEnabled; set => this.RaiseAndSetIfChanged(ref _isElectricityPriceEnabled, value); }

    private void UpdateScenario()
    {
        ScenarioManager.CurrentScenario = _selectedScenario;
    }

    private void RefreshDataAndChart()
    {
        UpdateScenario();
        SyncOptimizationPeriod();
        Optimizer.SetMaintenanceData(SelectedUnit?.Name ?? string.Empty, MaintenanceHours);
        ResultDataManager.GetResultData();
        Visualizer.Model.UpdateData();
        UpdateChart();
    }

    // =# CHART UPDATER #=
    // core logic to refresh graph and grey out the checkboxes
    private void UpdateChart()
    {
        bool hasEnergy = ShowHeatDemand || ShowHeatProduction || ShowElectricity;
        bool hasFinance = ShowProductionCosts || ShowElectricityPrice;
        bool hasCo2 = ShowCo2Emissions;
        bool hasFuel = ShowFuelConsumption;

        int activeGroups = (hasEnergy ? 1 : 0) + (hasFinance ? 1 : 0) + (hasCo2 ? 1 : 0) + (hasFuel ? 1 : 0);

        IsEnergyEnabled = (activeGroups < 2) || hasEnergy;
        IsFinanceEnabled = (activeGroups < 2) || hasFinance;
        IsCo2Enabled = (activeGroups < 2) || hasCo2;
        IsFuelEnabled = (activeGroups < 2) || hasFuel;

        // Electricity options are only enabled in Heat & Electricity scenario
        bool isElectricityScenario = SelectedScenario == Scenario.HeatAndElectricity;
        IsElectricityEnabled = isElectricityScenario && IsEnergyEnabled;
        IsElectricityPriceEnabled = isElectricityScenario && IsFinanceEnabled;

        var activeKinds = new List<DataKind>();
        if (ShowHeatDemand) activeKinds.Add(DataKind.HeatDemand);
        if (ShowHeatProduction) activeKinds.Add(DataKind.HeatProduction);
        if (ShowElectricity) activeKinds.Add(DataKind.Electricity);
        if (ShowElectricityPrice) activeKinds.Add(DataKind.ElectricityPrice);
        if (ShowCo2Emissions) activeKinds.Add(DataKind.Co2Emissions);
        if (ShowFuelConsumption) activeKinds.Add(DataKind.FuelConsumption);
        if (ShowProductionCosts) activeKinds.Add(DataKind.ProductionCosts);

        Visualizer.UpdatePlot(activeKinds, SelectedPeriod);
    }

    // =# UNIT CARDS PAGINATION #=
    // All loaded units keeps everything from json
    private List<UnitViewModel> _allUnits = new();

    // The 3 units currently shown on screen in the cards area
    private ObservableCollection<UnitViewModel> _visibleUnits = new();
    public ObservableCollection<UnitViewModel> VisibleUnits => _visibleUnits;

    // Which page we are on (0 = first 3 units, 1 = next 3, etc)
    private int _unitPageIndex = 0;

    // simple math checks if we can go back or forward so the arrows grey out automatically
    public bool CanGoPrev => _unitPageIndex > 0;
    public bool CanGoNext => (_unitPageIndex + 1) * 3 < _allUnits.Count;

    public void PreviousPage()
    {
        if (!CanGoPrev) return;
        _unitPageIndex--;
        RefreshVisibleUnits();
    }

    public void NextPage()
    {
        if (!CanGoNext) return;
        _unitPageIndex++;
        RefreshVisibleUnits();
    }

    private void RefreshVisibleUnits()
    {
        _visibleUnits.Clear();
        int start = _unitPageIndex * 3;
        
        // Skip jumps over previous pages and Take grabs exactly 3 for the current page
        var page = _allUnits.Skip(start).Take(3);
        
        foreach (var unit in page)
            _visibleUnits.Add(unit);

        this.RaisePropertyChanged(nameof(CanGoPrev));
        this.RaisePropertyChanged(nameof(CanGoNext));
    }

    private bool _isDarkTheme = true;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set { this.RaiseAndSetIfChanged(ref _isDarkTheme, value); ApplyTheme(); }
    }

    // The full list is still kept for the ComboBox in the left panel
    public ObservableCollection<UnitViewModel> Units { get; } = new();

    public MainWindowViewModel()
    {
        ApplyTheme();
        Visualizer.UpdateThemeColors(IsDarkTheme);
        LoadUnits();
        RefreshDataAndChart();
    }

    private void LoadUnits()
    {
        // just ask AssetManager for the units, it loads them from json automatically
        var units = AssetManager.GetDataForDataVisualizer().Item1;
        foreach (var data in units)
        {
            var vm = new UnitViewModel(
                data.Name,
                data.FuelName,
                data.MaxHeat,
                data.MaxElectricity,
                data.ProductionCost,
                data.Emissions,
                data.FuelConsumption,
                data.ImagePath ?? string.Empty
            );
            _allUnits.Add(vm);
            Units.Add(vm);
        }

        RefreshVisibleUnits();
    }

    // pushes the bright or dark mode deep into Avalonia internal settings
    private void ApplyTheme()
    {
        if (Application.Current != null)
            Application.Current.RequestedThemeVariant = IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            
        Visualizer?.UpdateThemeColors(IsDarkTheme);
    }
}