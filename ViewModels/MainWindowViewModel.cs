using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;
using ReactiveUI;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels;

public enum Scenario { Heat, HeatAndElectricity }
public enum Period { Winter, Summer }
public enum ChartOption { HeatConsumed }

public partial class MainWindowViewModel : ReactiveObject
{
    public Scenario SelectedScenario;
    //scenarios
    public bool IsHeatScenarioSelected
    {
        get => SelectedScenario == Scenario.Heat;
        set 
        { 
            if (value) 
            {
                SelectedScenario = Scenario.Heat;
                this.RaisePropertyChanged(nameof(IsHeatScenarioSelected));
                this.RaisePropertyChanged(nameof(IsHeatAndElecScenarioSelected));
            }
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
                this.RaisePropertyChanged(nameof(IsHeatScenarioSelected));
                this.RaisePropertyChanged(nameof(IsHeatAndElecScenarioSelected));
            }
        }
    }

    public Period SelectedPeriod;

    //periods
    public bool IsWinterSelected
    {
        get => SelectedPeriod == Period.Winter;
        set 
        { 
            if (value) 
            {
                SelectedPeriod = Period.Winter;
                this.RaisePropertyChanged(nameof(IsWinterSelected));
                this.RaisePropertyChanged(nameof(IsSummerSelected));
            }
        }
    }

    public bool IsSummerSelected
    {
        get => SelectedPeriod == Period.Summer;
        set 
        { 
            if (value) 
            {
                SelectedPeriod = Period.Summer;
                this.RaisePropertyChanged(nameof(IsWinterSelected));
                this.RaisePropertyChanged(nameof(IsSummerSelected));
            }
        }
    }


    //maintenance planning    
    private double _maintenanceHours = 30;
    public double MaintenanceHours
    {
        get => _maintenanceHours;
        set => this.RaiseAndSetIfChanged(ref _maintenanceHours, value);
    }

    //selected unit
    private UnitViewModel? _selectedUnit;
    public UnitViewModel? SelectedUnit
    {
        get => _selectedUnit;
        set => this.RaiseAndSetIfChanged(ref _selectedUnit, value);
    }

    //chart option
    private ChartOption _selectedChartOption = ChartOption.HeatConsumed;
    public ChartOption SelectedChartOption
    {
        get => _selectedChartOption;
        set => this.RaiseAndSetIfChanged(ref _selectedChartOption, value);
    }

    //theme
    private bool _isDarkTheme = true;
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            this.RaiseAndSetIfChanged(ref _isDarkTheme, value);
            ApplyTheme();
        }
    }

    //unit list
    public ObservableCollection<UnitViewModel> Units {get;} = new ObservableCollection<UnitViewModel>();

    public MainWindowViewModel()
    {
        LoadUnits();
    }

    private void LoadUnits()
    {
        Units.Add(new UnitViewModel("Gas Boiler 1", 3, 0, 550, 132, 1.05, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasBoiler.png"));
        Units.Add(new UnitViewModel("Gas Boiler 2", 2, 0, 510, 134, 1.08, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasBoiler.png"));
        Units.Add(new UnitViewModel("Gas Boiler 3", 4, 0, 560, 136, 1.09, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasBoiler.png"));
        Units.Add(new UnitViewModel("Oil Boiler 1", 6, 0, 680, 143, 1.18, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/OilBoiler.png"));
        Units.Add(new UnitViewModel("Gas Motor 1", 5.3, 0, 975, 227, 1.82, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasMotor.png"));
        Units.Add(new UnitViewModel("Electric Boiler 1", 6, -6, 15, 0, 0, "avares://Danfoss_Heat_Distribution_Optimizer/Assets/ElectricBoiler.png"));
    }

    private void ApplyTheme()
    {
        Application.Current!.RequestedThemeVariant = IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
    }
}
