using System.Windows;
using Microsoft.EntityFrameworkCore;
using PlantTracker.Data;
using PlantTracker.Wpf.ViewModels;

namespace PlantTracker.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();

        var context = new PlantTrackerDBContext();
        context.Database.Migrate();
        DBSeeder.Seed(context);

        var plantService = new PlantService(context);
        _viewModel = new MainViewModel(plantService);
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadPlantsAsync();
    }

}