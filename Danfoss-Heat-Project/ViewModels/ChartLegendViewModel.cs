using System.Collections.ObjectModel;
using ReactiveUI;

namespace Danfoss_Heat_Distribution_Optimizer.ViewModels
{
    public class LegendItem
    {
        public string Title { get; set; } = string.Empty;
        public Avalonia.Media.IBrush? Brush { get; set; }
    }

    public class ChartLegendViewModel : ViewModelBase
    {
        private ObservableCollection<LegendItem> _activeLegendItems = new();
        public ObservableCollection<LegendItem> ActiveLegendItems
        {
            get => _activeLegendItems;
            set => this.RaiseAndSetIfChanged(ref _activeLegendItems, value);
        }

        private bool _isLegendExpanded = false;
        public bool IsLegendExpanded
        {
            get => _isLegendExpanded;
            set => this.RaiseAndSetIfChanged(ref _isLegendExpanded, value);
        }

        public void ToggleLegend()
        {
            IsLegendExpanded = !IsLegendExpanded;
        }
    }
}
