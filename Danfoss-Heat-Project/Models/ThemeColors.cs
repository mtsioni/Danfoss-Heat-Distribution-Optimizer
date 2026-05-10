using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Danfoss_Heat_Distribution_Optimizer.ViewModels;

namespace Danfoss_Heat_Distribution_Optimizer.Models
{
    public class ThemeColors
    {
        public void UpdateThemeColors(PlotModel plotModel, bool isDarkTheme)
            {
                if (plotModel == null) return;
                var TextAxisColor = isDarkTheme ? OxyColor.Parse("#efebe5") : OxyColor.Parse("#5E0006");

                plotModel.TextColor = TextAxisColor;
                plotModel.PlotAreaBorderColor = TextAxisColor;

                foreach (var axis in plotModel.Axes)
                {
                    axis.TextColor = TextAxisColor;
                    axis.TitleColor = TextAxisColor;
                    axis.TicklineColor = TextAxisColor;
                    axis.AxislineColor = TextAxisColor;
                }

                plotModel.InvalidatePlot(false);
            }
    }
}    