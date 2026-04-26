using Avalonia.Headless.XUnit;
using Danfoss_Heat_Distribution_Optimizer.Services;

namespace Danfoss_Heat_Project.Tests.Integration
{
    public class SourceDataManagerIntegrationTests
    {
        [Fact]
        public void GetHeatDemand_ShouldNotBeNull()
        {
            // Act
            var heatDemand = SourceDataManager.GetHeatDemand();

            // Assert
            Assert.NotNull(heatDemand);
        }

        [Fact]
        public void GetElectricityPrice_ShouldNotBeNull()
        {
            // Act
            var electricityPrices = SourceDataManager.GetElectricityPrice();

            // Assert
            Assert.NotNull(electricityPrices);
        }

        [Fact]
        public void GetHeatDemand_ShouldContainData()
        {
            // Act
            var heatDemand = SourceDataManager.GetHeatDemand();

            // Assert
            Assert.NotNull(heatDemand);
            Assert.True(heatDemand.Values.Count > 0, "HeatDemand TimeSeries should contain at least one entry.");
        }

        [Fact]
        public void GetElectricityPrice_ShouldContainData()
        {
            // Act
            var electricityPrices = SourceDataManager.GetElectricityPrice();

            // Assert
            Assert.NotNull(electricityPrices);
            Assert.True(electricityPrices.Values.Count > 0, "ElectricityPrices TimeSeries should contain at least one entry.");
        }
    }
}
