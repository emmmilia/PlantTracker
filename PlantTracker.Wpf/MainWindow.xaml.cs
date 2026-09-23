using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using PlantTracker.Data;
using PlantTracker.Wpf.ViewModels;
using System.Windows.Threading;
using CommunityToolkit.WinUI.Notifications;
using System.Windows.Input;

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

    private void PreviousButton_Click(object sender, RoutedEventArgs e) => _viewModel.PreviousPlant();
    private void NextButton_Click(object sender, RoutedEventArgs e) => _viewModel.NextPlant();

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
        var dialog = new ConfirmDialog("delete forever?") { Owner = this };
        if (dialog.ShowDialog() == true)
        {
            string? photo = _viewModel.CurrentPlant?.PhotoPath;
            await _plantService.DeletePlantAsync(plantId);
            PhotoStorage.Delete(photo);
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
    private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left)
        {
            _viewModel.PreviousPlant();
            e.Handled = true;
        }
        else if (e.Key == Key.Right)
        {
            _viewModel.NextPlant();
            e.Handled = true;
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