using System.ComponentModel;
using System.Collections.ObjectModel;
using PlantTracker.Core;
using PlantTracker.Data;

namespace PlantTracker.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly PlantService _plantService;

        public ObservableCollection<Plant> Plants { get; set; } = new ObservableCollection<Plant>();

        public MainViewModel(PlantService plantService)
        {
            _plantService = plantService;
        }

        public async Task LoadPlantsAsync()
        {
            var plants = await _plantService.GetAllPlantsAsync();
            Plants.Clear();
            foreach (var plant in plants)
            {
                Plants.Add(plant);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}