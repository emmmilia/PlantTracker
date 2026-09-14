using PlantTracker.Data;
using PlantTracker.Wpf.ViewModels;
using System.Windows;

namespace PlantTracker.Wpf
{
    public partial class AddPlantWindow : Window
    {
        private readonly AddPlantViewModel _viewModel;
        public AddPlantWindow(PlantService plantService)
        {
            InitializeComponent();
            _viewModel = new AddPlantViewModel(plantService);
            this.DataContext = _viewModel;
        }
        private async void AddPlantWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadSpeciesAsync();
        }
        public AddPlantWindow()
        {
            InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e) 
        { 
            await _viewModel.SavePlantAsync(); this.Close(); 
        }

    }
}
