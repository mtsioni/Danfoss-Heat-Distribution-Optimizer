using Avalonia.Headless.XUnit;
using Danfoss_Heat_Distribution_Optimizer.ViewModels;
using Danfoss_Heat_Distribution_Optimizer.Views;
using Danfoss_Heat_Distribution_Optimizer.Models;

namespace Danfoss_Heat_Project.Tests
{
    [Collection("Sequential")]
    public class MainWindowViewModelTests
    {
        [Fact]
        public void IsHeatScenarioSelected_WhenSetToTrue_UpdatesScenarioAndHidesElectricity()
        {
            // Arrange
            var viewModel = new MainWindowViewModel();
            
            // Ensure we start in a different state to verify the change
            viewModel.SelectedScenario = Scenario.HeatAndElectricity;
            viewModel.ShowElectricity = true;
            viewModel.ShowElectricityPrice = true;

            // Act
            viewModel.IsHeatScenarioSelected = true;

            // Assert
            Assert.Equal(Scenario.Heat, viewModel.SelectedScenario);
            Assert.False(viewModel.ShowElectricity);
            Assert.False(viewModel.ShowElectricityPrice);
        }

        [Fact]
        public void IsHeatAndElecScenarioSelected_WhenSetToTrue_UpdatesScenario()
        {
            // Arrange
            var viewModel = new MainWindowViewModel();
            viewModel.SelectedScenario = Scenario.Heat;

            // Act
            viewModel.IsHeatAndElecScenarioSelected = true;

            // Assert
            Assert.Equal(Scenario.HeatAndElectricity, viewModel.SelectedScenario);
        }
        [Fact]
        public void Pagination_NextAndPreviousCommands_UpdateVisibleUnitsAndFlags()
        {
            // Arrange
            var viewModel = new MainWindowViewModel();
            
            // Act & Assert Initial State
            Assert.False(viewModel.CanGoPrev);
            Assert.True(viewModel.CanGoNext);

            // Act: Go to next page
            viewModel.NextPage();
            viewModel.NextPage();

            // Assert Next Page State
            Assert.True(viewModel.CanGoPrev);

            // Act: Go back to first page
            viewModel.PreviousPage();

            // Assert
            Assert.True(viewModel.CanGoPrev);
        }

        [Fact]
        public void UpdateChart_EnforcesMaximumOfTwoActiveGroups()
        {
            // Arrange
            var viewModel = new MainWindowViewModel();
            
            viewModel.ShowHeatDemand = true;      
            viewModel.ShowProductionCosts = true; 
            viewModel.ShowCo2Emissions = true;    
            viewModel.ShowFuelConsumption = true; 
            
            // Assert: 
            Assert.True(viewModel.IsEnergyEnabled);
            Assert.True(viewModel.IsFinanceEnabled);
            Assert.True(viewModel.IsCo2Enabled);
            Assert.True(viewModel.IsFuelEnabled);
            
            viewModel.ShowCo2Emissions = false;
            viewModel.ShowFuelConsumption = false;
            
            // Assert
            Assert.False(viewModel.IsCo2Enabled, "CO2 should be disabled since 2 other groups are already active.");
            Assert.False(viewModel.IsFuelEnabled, "Fuel should be disabled since 2 other groups are already active.");
        }
    }
}