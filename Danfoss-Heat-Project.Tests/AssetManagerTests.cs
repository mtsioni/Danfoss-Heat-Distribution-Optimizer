using Danfoss_Heat_Distribution_Optimizer.Services;
using Danfoss_Heat_Distribution_Optimizer.Data;

public class AssetManagerTest
{
    //Positive case
    [Fact]
    public void Initialize_ValidJson_LoadsData()
    {
        //Act
        AssetManager.Initialize();
        var units = AssetManager.GetDataForOptimizer();

        //Assert
        Assert.NotNull(units);
    }

    //Negative
    [Fact]
    public void Initialize_JsonDoesntExist_ThrowsException()
    {
        //Act
        Directory.SetCurrentDirectory(Path.GetTempPath());
        AssetsLoader.Initialize();
        var (units, grid, logo) = AssetsLoader.LoadGridData();

        //Assert
        Assert.Empty(units);
    }
}