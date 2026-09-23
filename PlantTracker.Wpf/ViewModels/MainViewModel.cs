using System.ComponentModel;
using System.Collections.ObjectModel;
using PlantTracker.Core;
using PlantTracker.Data;

namespace PlantTracker.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Plant> Plants { get; set; } = new ObservableCollection<Plant>();
        private readonly PlantService _plantService;
        private int _currentIndex;
        public Plant? currentPlant => Plants.Count > 0 ? Plants[_currentIndex] : null;
        public string PositionText => Plants.Count > 0 ? $"{_currentIndex + 1} / {Plants.Count}" : "0 / 0";
        public string NextWateringText 
        {
            get
            {
                if (currentPlant is null) return "";
                DateTime next = GetNextWatering(currentPlant);
                int days = (next.Date - DateTime.Today).Days;

                return days switch
                {
                    < 0 => $"late by {-days} day(s)!",
                    0 => "today",
                    1 => "tomorrow",
                    _ => $"in {days} days"
                };
            }
        }

        private static DateTime GetNextWatering(Plant plant)
        {
            int frequency = plant.CustomWateringFrequencyDays ?? plant.Species.wateringFrequency;
            return plant.LastWatered.AddDays(frequency);
        }
        public void NextPlant()
        {
            if (Plants.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % Plants.Count;
            RefreshCurrent();
        }

        public void PreviousPlant() 
        {
            if (Plants.Count == 0) return;
            _currentIndex = (_currentIndex - 1 + Plants.Count) % Plants.Count;
            RefreshCurrent();
        }

        private void RefreshCurrent() 
        {
            OnPropertyChanged(nameof(currentPlant));
            OnPropertyChanged(nameof(PositionText));
            OnPropertyChanged(nameof(NextWateringText));
        }

        public MainViewModel(PlantService plantService)
        {
            _plantService = plantService;
        }

        public async Task LoadPlantsAsync()
        {
            var plants = await _plantService.GetAllPlantsAsync();
            Plants.Clear();
            foreach (var plant in plants.OrderBy(GetNextWatering))
            {
                Plants.Add(plant);
            }
            if (_currentIndex >= Plants.Count)
                _currentIndex = Math.Max(0, Plants.Count - 1);
            RefreshCurrent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}