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
    public DataVisualizerViewModel Visualizer { get; } = new();

    public PlotModel CurrentPlotModel => Visualizer.CurrentPlotModel;

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
            this.RaisePropertyChanged(nameof(IsHeatScenarioSelected));
            this.RaisePropertyChanged(nameof(IsHeatAndElecScenarioSelected));
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
        }
    }

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

    // Chart checkboxes
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

    private bool _isEnergyEnabled = true;
    public bool IsEnergyEnabled { get => _isEnergyEnabled; set => this.RaiseAndSetIfChanged(ref _isEnergyEnabled, value); }
    
    private bool _isFinanceEnabled = true;
    public bool IsFinanceEnabled { get => _isFinanceEnabled; set => this.RaiseAndSetIfChanged(ref _isFinanceEnabled, value); }
    
    private bool _isEnvironmentEnabled = true;
    public bool IsEnvironmentEnabled { get => _isEnvironmentEnabled; set => this.RaiseAndSetIfChanged(ref _isEnvironmentEnabled, value); }

    private void UpdateChart()
    {
        bool hasEnergy = ShowHeatProduced || ShowHeatConsumed || ShowElectricityProduced || ShowElectricityConsumed || ShowElectricityPrice;
        bool hasFinance = ShowMoneyEarned || ShowMoneySpent || ShowProfit || ShowExpenses;
        bool hasEnvironment = ShowCo2Emissions || ShowFuelConsumption || ShowPrimaryEnergy;

        int activeGroups = (hasEnergy ? 1 : 0) + (hasFinance ? 1 : 0) + (hasEnvironment ? 1 : 0);

        IsEnergyEnabled = (activeGroups < 2) || hasEnergy;
        IsFinanceEnabled = (activeGroups < 2) || hasFinance;
        IsEnvironmentEnabled = (activeGroups < 2) || hasEnvironment;

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

    // All loaded units
    private List<UnitViewModel> _allUnits = new();

    // The 3 units currently shown on screen
    private ObservableCollection<UnitViewModel> _visibleUnits = new();
    public ObservableCollection<UnitViewModel> VisibleUnits => _visibleUnits;

    // Which page we are on (0 = first 3 units, 1 = next 3, etc)
    private int _unitPageIndex = 0;

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
        try
        {
            string jsonPath = FindAsset("ProductionUnits.json");
            AssetManager.Initialize(jsonPath, jsonPath, string.Empty);

            var units = AssetManager.GetDataForOptimizer();
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
        }
        catch
        {
            LoadFallbackUnits();
            return;
        }

        RefreshVisibleUnits();
    }

    private void LoadFallbackUnits()
    {
        _allUnits.Add(new UnitViewModel("GB1", "Gas", 3, 0, 510, 132, 1.05, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasBoiler.png"));
        _allUnits.Add(new UnitViewModel("GB2", "Gas", 2, 0, 540, 134, 1.08, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasBoiler.png"));
        _allUnits.Add(new UnitViewModel("GB3", "Gas", 4, 0, 580, 136, 1.09, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasBoiler.png"));
        _allUnits.Add(new UnitViewModel("OB1", "Oil",  6, 0, 690, 147, 1.18, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/OilBoiler.png"));
        _allUnits.Add(new UnitViewModel("GM1", "Gas", 5.3, 3.9, 975, 227, 1.82, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasMotor.png"));
        _allUnits.Add(new UnitViewModel("EB1", "Electric", 6, -6, 15, 0, 0, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/HeatingArea.png"));

        foreach (var u in _allUnits) Units.Add(u);
        RefreshVisibleUnits();
    }

    private string FindAsset(string filename)
    {
        string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", filename);
        if (File.Exists(rootPath)) return rootPath;

        string debugPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Assets", filename);
        if (File.Exists(debugPath)) return debugPath;

        return rootPath;
    }

    private void ApplyTheme()
    {
        if (Application.Current != null)
            Application.Current.RequestedThemeVariant = IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
            
        Visualizer?.UpdateThemeColors(IsDarkTheme);
    }
}