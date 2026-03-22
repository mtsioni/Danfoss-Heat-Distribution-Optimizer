using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace Danfoss_Heat_Distribution_Optimizer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // this sets the default theme to light when the app launched
        Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
    }

    // this function is called when the theme toggle is clicked and it changes the theme
    private void ThemeToggle_CheckedChanged(object? sender, RoutedEventArgs e)
    {
        var toggle = sender as ToggleSwitch;
        
        if (toggle.IsChecked == true)
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
        
        else
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }
    }
}