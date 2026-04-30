using Danfoss_Heat_Distribution_Optimizer.Services;
using Danfoss_Heat_Distribution_Optimizer.Data;
using Danfoss_Heat_Distribution_Optimizer.Models;
using System.Linq;
using System;
[Collection("Sequential")]
public class FunctionalTests
{
    [Fact]
    public void ScenarioManager_WhenHeatScenarioIsSelected_GivesCorrectDataToOptimizer()
    {
        //Arrange
        AssetManager.Initialize();
        var (units, grid, logo) = AssetManager.GetDataForDataVisualizer();
        ScenarioManager.CurrentScenario = Scenario.Heat;

        //Act
        var filteredList = ScenarioManager.GetFilteredList(units);

        //Assert
        Assert.NotEmpty(filteredList);
        Assert.Contains(filteredList, u => u.Name.Contains("Gas Boiler 1"));
        Assert.Contains(filteredList, u => u.Name.Contains("Gas Boiler 2"));
        Assert.Contains(filteredList, u => u.Name.Contains("Gas Boiler 3"));
        Assert.Contains(filteredList, u => u.Name.Contains("Oil Boiler 1"));
        Assert.DoesNotContain(filteredList, u => u.Name.Contains("Gas Motor 1"));
        Assert.DoesNotContain(filteredList, u => u.Name.Contains("Electric Boiler 1"));
    }

    [Fact]
    public void ScenarioManager_WhenHeatAndElectricityScenarioIsSelected_GivesCorrectDataToOptimizer()
    {
        //Arrange
        AssetManager.Initialize();
        var (units, grid, logo) = AssetManager.GetDataForDataVisualizer();
        ScenarioManager.CurrentScenario = Scenario.HeatAndElectricity;

        //Act
        var filteredList = ScenarioManager.GetFilteredList(units);

        //Assert
        Assert.NotEmpty(filteredList);
        Assert.Contains(filteredList, u => u.Name.Contains("Gas Boiler 1"));
        Assert.DoesNotContain(filteredList, u => u.Name.Contains("Gas Boiler 2"));
        Assert.Contains(filteredList, u => u.Name.Contains("Gas Boiler 3"));
        Assert.Contains(filteredList, u => u.Name.Contains("Gas Motor 1"));
        Assert.Contains(filteredList, u => u.Name.Contains("Electric Boiler 1"));
        Assert.DoesNotContain(filteredList, u => u.Name.Contains("Oil Boiler 1"));
    }

    [Fact]
    public void DataVisualizer_WhenWinterPeriodAndHeatAndElectricityScenarioIsSelected_ReceivesCorrectSourceDataForChart()
    {
        //Act 
        var allHeatDemand = SourceDataManager.GetHeatDemand();
        var allElectricityPrices = SourceDataManager.GetElectricityPrice();

        var winterDates = allHeatDemand.Values.Where(kvp => kvp.Key.Month == 1).OrderBy(kvp => kvp.Key).ToList();
        DateTime periodStart = winterDates.First().Key;
        DateTime periodEnd = winterDates.Last().Key;

        SourceDataManager.OptimizationPeriodStart = periodStart;
        SourceDataManager.OptimizationPeriodEnd = periodEnd;
        SourceDataManager.TimeResolution = 1;
        ScenarioManager.CurrentScenario = Scenario.HeatAndElectricity;

        List<double> chartHeatDemandData = new List<double>();
        List<double> chartElectricityPriceData = new List<double>();

        for (DateTime t = SourceDataManager.OptimizationPeriodStart; t <= SourceDataManager.OptimizationPeriodEnd; t = t.AddHours(SourceDataManager.TimeResolution))
        {
            chartHeatDemandData.Add(allHeatDemand[t]);
            chartElectricityPriceData.Add(allElectricityPrices[t]);
        }

        Assert.NotEmpty(chartHeatDemandData);
        Assert.NotEmpty(chartElectricityPriceData);
        Assert.Equal(chartHeatDemandData.Count, chartElectricityPriceData.Count);

        Assert.Contains(chartHeatDemandData, value => value > 0);
        Assert.Contains(chartElectricityPriceData, value => value > 0);
    }
}