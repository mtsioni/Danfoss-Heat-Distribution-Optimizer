using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Danfoss_Heat_Distribution_Optimizer.ViewModels;
using System;
using System.IO;
using System.Text.Json;

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
        //We can choose between the two approaches based on what we think it's better


        // Option 1: Hardcoded paths

        /*
        GasBoiler1.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasBoiler.png")));
        GasBoiler2.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasBoiler.png")));
        GasBoiler3.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasBoiler.png")));
        OilBoiler.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/OilBoiler.png")));
        GasMotor.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/GasMotor.png")));
        ElectricBoiler.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/ElectricBoiler.png")));
        */

        Logo.Source = new Bitmap(AssetLoader.Open(new Uri("avares://Danfoss_Heat_Distribution_Optimizer/Assets/Images/DanfossLogo.png")));


        //Option 2: Load from Paths.json (new approach)
        //     try
        //     {
        //         // Load image paths from Paths.json
        //         string pathsJson = File.ReadAllText("Assets/Paths.json");
        //         using (JsonDocument doc = JsonDocument.Parse(pathsJson))
        //         {
        //             var root = doc.RootElement;
        //             var images = root.GetProperty("images")[0];

        //             Logo.Source = new Bitmap(AssetLoader.Open(new Uri(images.GetProperty("danfossLogo").GetString())));

        //             // For individual boilers:
        //             // GasBoiler1.Source = new Bitmap(AssetLoader.Open(new Uri(images.GetProperty("gasBoiler").GetString())));
        //             // GasBoiler2.Source = new Bitmap(AssetLoader.Open(new Uri(images.GetProperty("gasBoiler").GetString())));
        //             // GasBoiler3.Source = new Bitmap(AssetLoader.Open(new Uri(images.GetProperty("gasBoiler").GetString())));
        //             // OilBoiler.Source = new Bitmap(AssetLoader.Open(new Uri(images.GetProperty("oilBoiler").GetString())));
        //             // GasMotor.Source = new Bitmap(AssetLoader.Open(new Uri(images.GetProperty("gasMotor").GetString())));
        //             // ElectricBoiler.Source = new Bitmap(AssetLoader.Open(new Uri(images.GetProperty("electricBoiler").GetString())));
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error loading images from Paths.json: {ex.Message}");
        //     }

    }


}