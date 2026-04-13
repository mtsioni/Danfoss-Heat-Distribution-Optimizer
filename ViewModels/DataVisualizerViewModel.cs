using System;
using System.Collections.Generic;
using Danfoss_Heat_Distribution_Optimizer.Models;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels
{
    public class DataVisualizerViewModel : ViewModelBase
    {
        // Using object as placeholder until other branch is merged
        private object _gridView;
        private object _heatView;
        private object _elecView;
        private object _finView;
        private object _envView;

        // TODO: Replace mock data with real ResultDataManager once that branch is merged
        private TimeSeries<double> _heatDemand;
        private TimeSeries<double> _electricityPrices;

        public DataVisualizerViewModel()
        {
            _heatDemand        = GenerateMockHeatDemand();
            _electricityPrices = GenerateMockElectricityPrices();
        }

        public void RefreshAll()
        {
            RenderHeat();
            RenderGrid();
            RenderElectricity();
            RenderEnvironmental();
            RenderFinance();
        }

        public void RenderHeat()
        {
            // TODO: Call _heatView.PlotHeat() once HeatDataView exists
        }

        public void RenderGrid()
        {
            // TODO: Call _gridView.DrawGrid() once GridConfigurationView exists
        }

        public void RenderElectricity()
        {
            // TODO: Call _elecView.PlotElectricity() once ElectricityDataView exists
        }

        public void RenderEnvironmental()
        {
            // TODO: Call _envView.PlotEnvironmental() once EnvironmentalView exists
        }

        public void RenderFinance()
        {
            // TODO: Call _finView.PlotFinancials() once FinancialView exists
        }

        private TimeSeries<double> GenerateMockHeatDemand()
        {
            var ts = new TimeSeries<double>();
            for (int i = 0; i < 24; i++)
                ts.Values[DateTime.Today.AddHours(i)] = 100 + i * 5;
            return ts;
        }

        private TimeSeries<double> GenerateMockElectricityPrices()
        {
            var ts = new TimeSeries<double>();
            for (int i = 0; i < 24; i++)
                ts.Values[DateTime.Today.AddHours(i)] = 50 + Math.Sin(i) * 10;
            return ts;
        }
    }
}