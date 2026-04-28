using Danfoss_Heat_Distribution_Optimizer.Services;
using Danfoss_Heat_Distribution_Optimizer.Models;
using Danfoss_Heat_Distribution_Optimizer.Services.Interfaces;

[Collection("Sequential")]
public class ResultDataManagerIntegrationTests
{
    //Positive case
    [Fact]
    public void ResultDatManger_GetResultData_ReturnsNonEmptyList()
    {
        //Arrange
        Optimizer.OptimizationPeriodStart = new DateTime(2026, 01, 05);
        Optimizer.OptimizationPeriodEnd = new DateTime(2026, 01, 18);
        string testProjectDirectory = Directory.GetCurrentDirectory();
        string mainProjectDirectory = Path.Combine(testProjectDirectory, "..", "Danfoss-Heat-Project");
        if (!File.Exists(Path.Combine(mainProjectDirectory, "Assets", "Paths.json")))
            mainProjectDirectory = Path.Combine(testProjectDirectory, "..", "..", "..", "..", "Danfoss-Heat-Project");
        Directory.SetCurrentDirectory(mainProjectDirectory);

        //Act
        var result = ResultDataManager.GetResultData();
        //Assert
        Assert.NotEmpty(result);
        //Cleanup
        Optimizer.OptimizationPeriodStart = new();
        Optimizer.OptimizationPeriodEnd = new();
        Directory.SetCurrentDirectory(testProjectDirectory);
    }
}