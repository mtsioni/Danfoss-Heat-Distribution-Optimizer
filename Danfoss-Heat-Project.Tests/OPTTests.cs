using Danfoss_Heat_Distribution_Optimizer.Services;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

[Collection("Sequential")]
public class OptimizerIntegrationTests
{
    //Positive case
    [Fact]
    public void Optimizer_GetResultData_ScenarioHeat_ReturnsListIOptimizedUnit()
    {
        //Arrange
        Optimizer.OptimizationPeriodStart = new DateTime(2026, 01, 05);
        Optimizer.OptimizationPeriodEnd = new DateTime(2026, 01, 18);
        //Act
        var result = Optimizer.GetResultData();
        //Assert
        Assert.True(result is List<IOptimizedUnit>);
        //Cleanup
        Optimizer.OptimizationPeriodStart = new();
        Optimizer.OptimizationPeriodEnd = new();
    }
    //Positive case
    [Fact]
    public void Optimizer_GetResultData_ScenarioHeatAndElectricity_ReturnsListIOptimizedUnit()
    {
        //Arrange
        ScenarioManager.CurrentScenario = Scenario.HeatAndElectricity;
        Optimizer.OptimizationPeriodStart = new DateTime(2026, 01, 05);
        Optimizer.OptimizationPeriodEnd = new DateTime(2026, 01, 18);
        //Act
        var result = Optimizer.GetResultData();
        //Assert
        Assert.True(result is List<IOptimizedUnit>);
        //Cleanup
        ScenarioManager.CurrentScenario = Scenario.Heat;
        Optimizer.OptimizationPeriodStart = new();
        Optimizer.OptimizationPeriodEnd = new();
    }
    //Positive case
    [Fact]
    public void Optimizer_GetResultData_ScenarioHeat_ReturnsCorrectUnits()
    {
        //Arrange
        List<string> names = new(){"Gas Boiler 1", "Gas Boiler 2", "Gas Boiler 3", "Oil Boiler 1"};
        Optimizer.OptimizationPeriodStart = new DateTime(2026, 01, 05);
        Optimizer.OptimizationPeriodEnd = new DateTime(2026, 01, 18);
        //Act
        var result = Optimizer.GetResultData();
        //Assert
        Assert.Equal(4, result.Count);
        for (int i = 0; i < result.Count; i++)
        {
            Assert.Contains(result[i].Name, names);
            if (names.Contains(result[i].Name))
                names.Remove(result[i].Name);
        }
        //Cleanup
        Optimizer.OptimizationPeriodStart = new();
        Optimizer.OptimizationPeriodEnd = new();
    }
    //Positive case
    [Fact]
    public void Optimizer_GetResultData_ScenarioHeatAndElectricity_ReturnsCorrectUnits()
    {
        //Arrange
        ScenarioManager.CurrentScenario = Scenario.HeatAndElectricity;
        List<string> names = new(){"Gas Boiler 1", "Gas Boiler 3", "Gas Motor 1", "Electric Boiler 1"};
        Optimizer.OptimizationPeriodStart = new DateTime(2026, 01, 05);
        Optimizer.OptimizationPeriodEnd = new DateTime(2026, 01, 18);

        //Act
        var result = Optimizer.GetResultData();
        
        //Assert
        Assert.Equal(4, result.Count);
        for (int i = 0; i < result.Count; i++)
        {
            Assert.Contains(result[i].Name, names);
            if (names.Contains(result[i].Name))
                names.Remove(result[i].Name);
        }
        //Cleanup
        ScenarioManager.CurrentScenario = Scenario.Heat;
        Optimizer.OptimizationPeriodStart = new();
        Optimizer.OptimizationPeriodEnd = new();
    }
}