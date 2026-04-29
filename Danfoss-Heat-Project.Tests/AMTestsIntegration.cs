using Danfoss_Heat_Distribution_Optimizer.Services;
using Danfoss_Heat_Distribution_Optimizer.Data;

[Collection("Sequential")]
public class AssetManagerIntegrationTests
{
    //Positive case
    [Fact]
    public void Initialize_ValidJson_LoadsData()
    {
        //Arrange
        AssetManager.Initialize();
        
        //Act
        var units = AssetManager.GetDataForOptimizer();

        //Assert
        Assert.NotEmpty(units);
    }

    //Negative
    [Fact]
    public void Initialize_JsonDoesntExist_ThrowsException()
    {
        //Arrange
        string originalDir = Directory.GetCurrentDirectory();
        
        try
        {
            //Act
            Directory.SetCurrentDirectory(Path.GetTempPath());
            AssetsLoader.Initialize();
            var (units, grid, logo) = AssetsLoader.LoadGridData();

            //Assert
            Assert.Empty(units);
        }
        finally
        {
            //Cleanup
            Directory.SetCurrentDirectory(originalDir);
        }
    }
}