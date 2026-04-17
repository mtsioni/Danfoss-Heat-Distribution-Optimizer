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

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    // visualizer holds all the graph logic so we dont clutter this file too much
    public DataVisualizerViewModel Visualizer { get; } = new();

    public PlotModel CurrentPlotModel => Visualizer.CurrentPlotModel;

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
            if (value) SelectedScenario = Scenario.Heat;
            // force electricity toggles off so traces drop from graph immediately
            ShowElectricityProduced = false;
            ShowElectricityConsumed = false;
            ShowElectricityPrice = false;
            this.RaisePropertyChanged(nameof(IsHeatScenarioSelected));
            this.RaisePropertyChanged(nameof(IsHeatAndElecScenarioSelected));
            this.RaisePropertyChanged(nameof(IsElectricityEnabled));
            UpdateChart();
        }
    }

    public bool IsHeatAndElecScenarioSelected
    {
        get => SelectedScenario == Scenario.HeatAndElectricity;
        set
        {
            if (value) SelectedScenario = Scenario.HeatAndElectricity;
            this.RaisePropertyChanged(nameof(IsHeatScenarioSelected));
            this.RaisePropertyChanged(nameof(IsHeatAndElecScenarioSelected));
            this.RaisePropertyChanged(nameof(IsElectricityEnabled));
            UpdateChart();
        }
    }

    // electricity checkboxes are only available when Heat & Electricity scenario is selected
    // and the group they belong to is not locked out by the 2-group limit
    public bool IsElectricityEnabled => SelectedScenario == Scenario.HeatAndElectricity;

    private Period _selectedPeriod = Period.Winter;
    public Period SelectedPeriod
    {
        get => _selectedPeriod;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPeriod, value);
            UpdateChart();
            this.RaisePropertyChanged(nameof(IsWinterSelected));
            this.RaisePropertyChanged(nameof(IsSummerSelected));
        }
    }

    public bool IsWinterSelected { get => SelectedPeriod == Period.Winter; set { if (value) SelectedPeriod = Period.Winter; } }
    public bool IsSummerSelected { get => SelectedPeriod == Period.Summer; set { if (value) SelectedPeriod = Period.Summer; } }

    private double _maintenanceHours = 30;
    public double MaintenanceHours { get => _maintenanceHours; set => this.RaiseAndSetIfChanged(ref _maintenanceHours, value); }

    private UnitViewModel? _selectedUnit;
    public UnitViewModel? SelectedUnit { get => _selectedUnit; set => this.RaiseAndSetIfChanged(ref _selectedUnit, value); }

    private bool _showHeatProduced = true;
    public bool ShowHeatProduced { get => _showHeatProduced; set { this.RaiseAndSetIfChanged(ref _showHeatProduced, value); UpdateChart(); } }

    private bool _showHeatConsumed = true;
    public bool ShowHeatConsumed { get => _showHeatConsumed; set { this.RaiseAndSetIfChanged(ref _showHeatConsumed, value); UpdateChart(); } }

    private bool _showElectricityProduced;
    public bool ShowElectricityProduced { get => _showElectricityProduced; set { this.RaiseAndSetIfChanged(ref _showElectricityProduced, value); UpdateChart(); } }

    private bool _showElectricityConsumed;
    public bool ShowElectricityConsumed { get => _showElectricityConsumed; set { this.RaiseAndSetIfChanged(ref _showElectricityConsumed, value); UpdateChart(); } }

    private bool _showElectricityPrice;
    public bool ShowElectricityPrice { get => _showElectricityPrice; set { this.RaiseAndSetIfChanged(ref _showElectricityPrice, value); UpdateChart(); } }

    private bool _showMoneyEarned;
    public bool ShowMoneyEarned { get => _showMoneyEarned; set { this.RaiseAndSetIfChanged(ref _showMoneyEarned, value); UpdateChart(); } }

    private bool _showMoneySpent;
    public bool ShowMoneySpent { get => _showMoneySpent; set { this.RaiseAndSetIfChanged(ref _showMoneySpent, value); UpdateChart(); } }

    private bool _showProfit;
    public bool ShowProfit { get => _showProfit; set { this.RaiseAndSetIfChanged(ref _showProfit, value); UpdateChart(); } }

    private bool _showExpenses;
    public bool ShowExpenses { get => _showExpenses; set { this.RaiseAndSetIfChanged(ref _showExpenses, value); UpdateChart(); } }

    private bool _showCo2Emissions;
    public bool ShowCo2Emissions { get => _showCo2Emissions; set { this.RaiseAndSetIfChanged(ref _showCo2Emissions, value); UpdateChart(); } }

    private bool _showFuelConsumption;
    public bool ShowFuelConsumption { get => _showFuelConsumption; set { this.RaiseAndSetIfChanged(ref _showFuelConsumption, value); UpdateChart(); } }

    private bool _showPrimaryEnergy;
    public bool ShowPrimaryEnergy { get => _showPrimaryEnergy; set { this.RaiseAndSetIfChanged(ref _showPrimaryEnergy, value); UpdateChart(); } }

    // =# GROUP ENABLE FLAGS #=
    // each of the 5 unit groups can be locked when 2 groups are already active
    private bool _isEnergyGroupEnabled = true;
    public bool IsEnergyGroupEnabled { get => _isEnergyGroupEnabled; set => this.RaiseAndSetIfChanged(ref _isEnergyGroupEnabled, value); }

    private bool _isFinanceGroupEnabled = true;
    public bool IsFinanceGroupEnabled { get => _isFinanceGroupEnabled; set => this.RaiseAndSetIfChanged(ref _isFinanceGroupEnabled, value); }

    private bool _isCo2GroupEnabled = true;
    public bool IsCo2GroupEnabled { get => _isCo2GroupEnabled; set => this.RaiseAndSetIfChanged(ref _isCo2GroupEnabled, value); }

    private bool _isFuelGroupEnabled = true;
    public bool IsFuelGroupEnabled { get => _isFuelGroupEnabled; set => this.RaiseAndSetIfChanged(ref _isFuelGroupEnabled, value); }

    private bool _isPrimaryEnergyGroupEnabled = true;
    public bool IsPrimaryEnergyGroupEnabled { get => _isPrimaryEnergyGroupEnabled; set => this.RaiseAndSetIfChanged(ref _isPrimaryEnergyGroupEnabled, value); }

    // =# COMBINED ENABLE PROPERTIES #=
    // the electricity checkboxes need both the scenario gate AND the group gate to be open
    // Electricity Produced/Consumed live in the Energy group
    public bool IsElecProdEnabled => IsElectricityEnabled && IsEnergyGroupEnabled;
    public bool IsElecConsEnabled => IsElectricityEnabled && IsEnergyGroupEnabled;
    // Electricity Price lives in the Financial group
    public bool IsElecPriceEnabled => IsElectricityEnabled && IsFinanceGroupEnabled;

    // =# CHART UPDATER #=
    // core logic to refresh graph and grey out the checkboxes based on 5 distinct unit groups
    // Group 1 - Energy (MW/MWh): HeatProduced, HeatConsumed, ElectricityProduced, ElectricityConsumed
    // Group 2 - Financial (DKK): Profit, Expenses, ElectricityPrice, MoneyEarned, MoneySpent
    // Group 3 - CO2 Emissions (kg)
    // Group 4 - Fuel Consumption (MWh)
    // Group 5 - Primary Energy (MW)
    private void UpdateChart()
    {
        bool hasEnergy = ShowHeatProduced || ShowHeatConsumed || ShowElectricityProduced || ShowElectricityConsumed;
        bool hasFinance = ShowMoneyEarned || ShowMoneySpent || ShowProfit || ShowExpenses || ShowElectricityPrice;
        bool hasCo2 = ShowCo2Emissions;
        bool hasFuel = ShowFuelConsumption;
        bool hasPrimary = ShowPrimaryEnergy;

        int activeGroups = (hasEnergy ? 1 : 0) + (hasFinance ? 1 : 0)
                         + (hasCo2 ? 1 : 0) + (hasFuel ? 1 : 0) + (hasPrimary ? 1 : 0);

        // if 2 groups are occupied, lock out every group that isnt already in use
        IsEnergyGroupEnabled    = (activeGroups < 2) || hasEnergy;
        IsFinanceGroupEnabled   = (activeGroups < 2) || hasFinance;
        IsCo2GroupEnabled       = (activeGroups < 2) || hasCo2;
        IsFuelGroupEnabled      = (activeGroups < 2) || hasFuel;
        IsPrimaryEnergyGroupEnabled = (activeGroups < 2) || hasPrimary;

        // raise the combined enable properties so the UI refreshes the electricity checkboxes
        this.RaisePropertyChanged(nameof(IsElecProdEnabled));
        this.RaisePropertyChanged(nameof(IsElecConsEnabled));
        this.RaisePropertyChanged(nameof(IsElecPriceEnabled));

        var active = new List<DataKind>();
        if (ShowHeatProduced) active.Add(DataKind.HeatProduced);
        if (ShowHeatConsumed) active.Add(DataKind.HeatConsumed);
        if (ShowElectricityProduced) active.Add(DataKind.ElectricityProduced);
        if (ShowElectricityConsumed) active.Add(DataKind.ElectricityConsumed);
        if (ShowElectricityPrice) active.Add(DataKind.ElectricityPrice);
        if (ShowMoneyEarned) active.Add(DataKind.MoneyEarned);
        if (ShowMoneySpent) active.Add(DataKind.MoneySpent);
        if (ShowProfit) active.Add(DataKind.Profit);
        if (ShowExpenses) active.Add(DataKind.Expenses);
        if (ShowCo2Emissions) active.Add(DataKind.Co2Emissions);
        if (ShowFuelConsumption) active.Add(DataKind.FuelConsumption);
        if (ShowPrimaryEnergy) active.Add(DataKind.PrimaryEnergy);

        Visualizer.UpdatePlot(active, SelectedPeriod);
        this.RaisePropertyChanged(nameof(CurrentPlotModel));
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
        Visualizer.UpdateThemeColors(IsDarkTheme);
        LoadUnits();
        UpdateChart();
    }

    private void LoadUnits()
    {
        string jsonPath = FindAsset("ProductionUnits.json");
        AssetManager.Initialize(jsonPath, jsonPath, string.Empty);

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

    // finds where the assets are placed depending if we are debugging or running production
    private string FindAsset(string filename)
    {
        string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", filename);
        if (File.Exists(rootPath)) return rootPath;

        string debugPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Assets", filename);
        if (File.Exists(debugPath)) return debugPath;

        return rootPath;
    }

    // pushes the bright or dark mode deep into Avalonia internal settings
    private void ApplyTheme()
    {
        if (Application.Current != null)
            Application.Current.RequestedThemeVariant = IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            
        Visualizer?.UpdateThemeColors(IsDarkTheme);
    }
}