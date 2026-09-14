using System.Windows;
using Microsoft.EntityFrameworkCore;
using PlantTracker.Data;
using PlantTracker.Wpf.ViewModels;

namespace PlantTracker.Wpf;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    public PlantService _plantService;
    public MainWindow()
    {
        InitializeComponent();

        var context = new PlantTrackerDBContext();
        context.Database.Migrate();
        DBSeeder.Seed(context);

        _plantService = new PlantService(context);  
        _viewModel = new MainViewModel(_plantService);
        this.DataContext = _viewModel;
    }

    private async void AddPlantButton_Click(object sender, RoutedEventArgs e) 
    { 
        var addWindow = new AddPlantWindow(_plantService); 
        addWindow.ShowDialog();
        await _viewModel.LoadPlantsAsync();
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadPlantsAsync();
    }

}