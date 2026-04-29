using Danfoss_Heat_Distribution_Optimizer.ViewModels;
using Danfoss_Heat_Distribution_Optimizer.Models;
using OxyPlot.Axes;
using System.Collections.Generic;
using System.Linq;

namespace Danfoss_Heat_Project.Tests;

public class DataVisualizerViewModelTests
{
    [Fact]
    public void ToggleLegend_FlipsIsLegendExpanded_Correctly()
    {
        // Arrange
        var viewModel = new DataVisualizerViewModel();
        Assert.False(viewModel.IsLegendExpanded);

        // Act
        viewModel.ToggleLegend();

        // Assert
        Assert.True(viewModel.IsLegendExpanded);

        // Act again
        viewModel.ToggleLegend();

        // Assert
        Assert.False(viewModel.IsLegendExpanded);
    }

    [Fact]
    public void UpdateThemeColors_DarkTheme_SetsCorrectHexColors()
    {
        // Arrange
        var viewModel = new DataVisualizerViewModel();

        // Act - Switch to Dark Theme
        viewModel.UpdateThemeColors(isDarkTheme: true);

        // Assert
        Assert.Equal(239, viewModel.CurrentPlotModel.TextColor.R);
        Assert.Equal(235, viewModel.CurrentPlotModel.TextColor.G);
        Assert.Equal(229, viewModel.CurrentPlotModel.TextColor.B);
    }

    [Fact]
    public void UpdateThemeColors_LightTheme_SetsCorrectHexColors()
    {
        // Arrange
        var viewModel = new DataVisualizerViewModel();

        // Act - Switch to Light Theme
        viewModel.UpdateThemeColors(isDarkTheme: false);

        // Assert
        Assert.Equal(94, viewModel.CurrentPlotModel.TextColor.R);
        Assert.Equal(0, viewModel.CurrentPlotModel.TextColor.G);
        Assert.Equal(6, viewModel.CurrentPlotModel.TextColor.B);
    }

    [Fact]
    public void UpdatePlot_WithTwoDataGroups_ConfiguresLeftAndRightAxes()
    {
        // Arrange
        var viewModel = new DataVisualizerViewModel();
        
        // We select Heat Demand (Energy group) and Production Costs (Financial group)
        var activeKinds = new List<DataKind> { DataKind.HeatDemand, DataKind.ProductionCosts };

        // Act
        viewModel.UpdatePlot(activeKinds, Period.Winter);

        // Assert
        var leftAxis = viewModel.CurrentPlotModel.Axes.First(a => a.Key == "LeftAxis");
        var rightAxis = viewModel.CurrentPlotModel.Axes.First(a => a.Key == "RightAxis");

        // Energy should be on the left, Financial on the right
        Assert.Equal("Energy (MW, MWh)", leftAxis.Title);
        Assert.True(leftAxis.IsAxisVisible);

        Assert.Equal("Financial (DKK)", rightAxis.Title);
        Assert.True(rightAxis.IsAxisVisible);
    }

    [Fact]
    public void UpdatePlot_WithOneDataGroup_HidesRightAxis()
    {
        // Arrange
        var viewModel = new DataVisualizerViewModel();
        
        // We select ONLY CO2 Emissions
        var activeKinds = new List<DataKind> { DataKind.Co2Emissions };

        // Act
        viewModel.UpdatePlot(activeKinds, Period.Summer);

        // Assert
        var leftAxis = viewModel.CurrentPlotModel.Axes.First(a => a.Key == "LeftAxis");
        var rightAxis = viewModel.CurrentPlotModel.Axes.First(a => a.Key == "RightAxis");

        Assert.Equal("CO2 Emissions (kg)", leftAxis.Title);
        Assert.True(leftAxis.IsAxisVisible);
        
        Assert.False(rightAxis.IsAxisVisible);
    }

    [Fact]
    public void GetAggregatedData_ForWinterPeriod_OnlyReturnsJanuaryData()
    {
        // Arrange
        var viewModel = new DataVisualizerViewModel();
        
        // Act 
        var winterDataDict = viewModel.GetAggregatedData(DataKind.HeatDemand, Period.Winter);

        // Assert
        foreach (var timestamp in winterDataDict.Keys)
        {
            Assert.Equal(1, timestamp.Month);
        }
    }

    [Fact]
    public void GetAggregatedData_ForSummerPeriod_OnlyReturnsSeptemberData()
    {
        // Arrange
        var viewModel = new DataVisualizerViewModel();
        
        // Act 
        var summerDataDict = viewModel.GetAggregatedData(DataKind.HeatDemand, Period.Summer);

        // Assert
        foreach (var timestamp in summerDataDict.Keys)
        {
            Assert.Equal(9, timestamp.Month);
        }
    }
}