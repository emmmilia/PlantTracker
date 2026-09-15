using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using PlantTracker.Data;
using PlantTracker.Wpf.ViewModels;
using System.Windows.Threading;
using CommunityToolkit.WinUI.Notifications;

namespace PlantTracker.Wpf;

public partial class MainWindow : Window
{
    private DispatcherTimer _wateringCheckTimer;
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

    private async void WaterButton_Click(object sender, RoutedEventArgs e) 
    {
        var button = (Button)sender;
        int plantId = (int)button.CommandParameter;
        await _plantService.WaterPlantAsync(plantId, "");
        await _viewModel.LoadPlantsAsync();
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e) 
    {
        var button = (Button)sender;
        int plantId = (int)button.CommandParameter;
        var result = MessageBox.Show("delete forever?", "delete", MessageBoxButton.YesNo);
        if(result == MessageBoxResult.Yes) 
        {
            await _plantService.DeletePlantAsync(plantId);
            await _viewModel.LoadPlantsAsync();
        }
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e) 
    {
        var button = (Button)sender;
        int plantId = (int)button.CommandParameter;
        var plant = await _plantService.GetPlantByIdAsync(plantId);
        var editWindow = new AddPlantWindow(_plantService, plant);
        editWindow.ShowDialog();
        await _viewModel.LoadPlantsAsync();
    }

    private async void WateringCheckTimer_Tick(object sender, EventArgs e) 
    {
        var plants = await _plantService.GetAllPlantsAsync();
        var thirstyPlants = plants.Where(p => p.CalculateStatus() >= Core.PlantStatus.Thirsty).ToList();
        if (thirstyPlants.Any()) 
        {
            string plantNames = string.Join(",", thirstyPlants.Select(p => p.Name));

            new ToastContentBuilder()
                .AddText("💧 time to water!!")
                .AddText($"{thirstyPlants.Count} plant/s need water: {plantNames}")
                .Show();
        }
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _wateringCheckTimer = new DispatcherTimer();
        _wateringCheckTimer.Interval = TimeSpan.FromHours(4);
        _wateringCheckTimer.Tick += WateringCheckTimer_Tick;
        _wateringCheckTimer.Start();
        await _viewModel.LoadPlantsAsync();
    }

}