using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Danfoss_Heat_Distribution_Optimizer.ViewModels;
using System;

namespace Danfoss_Heat_Distribution_Optimizer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        // this sets the default theme to light when the app launched
        //Application.Current!.RequestedThemeVariant = ThemeVariant.Light;

        ViewImages();        
    }

    /*
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
    */
    public void ViewImages()
    {
        /*
        GasBoiler1.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasBoiler.png")));
        GasBoiler2.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasBoiler.png")));
        GasBoiler3.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasBoiler.png")));
        OilBoiler.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/OilBoiler.png")));
        GasMotor.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/GasMotor.png")));
        ElectricBoiler.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/ElectricBoiler.png")));
        */
        Logo.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/DanfossLogo.png")));
    }
    
}