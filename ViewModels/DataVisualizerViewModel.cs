using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.ViewModels;
using Danfoss_Heat_Distribution_Optimizer.ViewModels.DataVisualizerSubclasses;

namespace VášProjekt.ViewModels.DataVisualizerSubclasses
{
    public class DataVisualizerViewModel : ViewModelBase
    {
        public GridConfigurationViewModel GridConfigVM { get; }
        public HeatDataViewModel HeatDataVM { get; }
        public ElectricityDataViewModel ElectricityDataVM { get; }
        public FinancialViewModel FinancialVM { get; }
        public EnvironmentalViewModel EnvironmentalVM { get; }

        private object _currentGraph;
        public object CurrentGraph
        {
            get => _currentGraph;
            set 
            {
                _currentGraph = value;
                OnPropertyChanged(nameof(CurrentGraph)); 
            }
        }

        public DataVisualizerViewModel()
        {
            GridConfigVM = new GridConfigurationViewModel();
            HeatDataVM = new HeatDataViewModel();
            ElectricityDataVM = new ElectricityDataViewModel();
            FinancialVM = new FinancialViewModel();
            EnvironmentalVM = new EnvironmentalViewModel();

            CurrentGraph = GridConfigVM; 
        }

        public void RenderGrid()
        {
            CurrentGraph = GridConfigVM;
        }

        public void RenderHeat()
        {
            CurrentGraph = HeatDataVM;
        }

        public void RenderElectricity()
        {
            CurrentGraph = ElectricityDataVM;
        }

        public void RenderFinance()
        {
            CurrentGraph = FinancialVM;
        }

        public void RenderEnvironmental()
        {
            CurrentGraph = EnvironmentalVM;
        }

        public void RefreshAll()
        {
            
        }
    }
}