using PlantTracker.Core;
using PlantTracker.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantTracker.Wpf.ViewModels
{
    public class AddPlantViewModel : INotifyPropertyChanged
    {
        private readonly PlantService _plantService;
        public event PropertyChangedEventHandler? PropertyChanged;
        private Plant _editingPlant;
        private string? _photoPath;
        private string? _originalPhotoPath;
        public bool IsSaved { get; private set; }
        public string? PhotoPath
        {
            get => _photoPath;
            set { _photoPath = value; OnPropertyChanged(nameof(PhotoPath)); }
        }
        public ObservableCollection<Species> AvailableSpecies { get; set; } = new ObservableCollection<Species>();

        public AddPlantViewModel(PlantService plantService, Plant plantToEdit) : this(plantService) 
        {   
            _editingPlant = plantToEdit;
            PhotoPath = plantToEdit.PhotoPath;
            Name = plantToEdit.Name;
            Location = plantToEdit.Location;
            Notes = plantToEdit.Notes;
            _originalPhotoPath = plantToEdit.PhotoPath;
            CustomWateringFrequencyDaysInput = plantToEdit.CustomWateringFrequencyDays?.ToString() ?? "";
            SelectedSpecies = plantToEdit.Species; 
        }

        public AddPlantViewModel(PlantService plantService)
        {
            _plantService = plantService;
        }
        public void SetNewPhoto(string sourcePath)
        {
            // si ya habían elegido otra foto en esta ventana, esa ya no sirve
            if (PhotoPath != _originalPhotoPath)
                PhotoStorage.Delete(PhotoPath);

            PhotoPath = PhotoStorage.Save(sourcePath);
        }

        public void DiscardUnsavedPhoto()
        {
            if (PhotoPath != _originalPhotoPath)
                PhotoStorage.Delete(PhotoPath);
        }

        public async Task LoadSpeciesAsync()
        {
            var species = await _plantService.GetAllSpeciesAsync();
            AvailableSpecies.Clear();
            foreach (var spec in species)
            {
                AvailableSpecies.Add(spec);
            }
        }

        public async Task SavePlantAsync()
        {
            if (SelectedSpecies == null)
            {
                throw new InvalidOperationException("no species selected.");
            }
            int? customFrequency = null;
            if (!string.IsNullOrWhiteSpace(CustomWateringFrequencyDaysInput))
            {
                if (int.TryParse(CustomWateringFrequencyDaysInput, out int parsed))
                {
                    customFrequency = parsed;
                }
                else
                {
                    throw new FormatException("custom watering frequency must be a valid integer.");
                }
            }
            //chequear si la estan editando
            if (_editingPlant != null)
            {   //si la estan editando actualizar la planta existente
                _editingPlant.Name = Name;
                _editingPlant.Location = Location;
                _editingPlant.Notes = Notes;
                _editingPlant.SpeciesId = SelectedSpecies.Id;
                _editingPlant.Species = SelectedSpecies;
                _editingPlant.CustomWateringFrequencyDays = customFrequency;
                _editingPlant.PlantStatus = _editingPlant.CalculateStatus();
                _editingPlant.PhotoPath = PhotoPath;

                await _plantService.UpdatePlantAsync(_editingPlant);
                if (_originalPhotoPath != PhotoPath)
                    PhotoStorage.Delete(_originalPhotoPath);
            }
            else
            { //sino crear la planta nueva
                var newPlant = new Plant()
                {
                    Name = Name,
                    Location = Location,
                    Notes = Notes,
                    SpeciesId = SelectedSpecies.Id,
                    CustomWateringFrequencyDays = customFrequency,
                    PhotoPath = PhotoPath,
                    LastWatered = DateTime.Now,
                    DateAdded = DateTime.Now,
                    StatusLastUpdated = DateTime.Now
                };
                newPlant.Species = SelectedSpecies; // la especie entera para calcular el status :p
                newPlant.PlantStatus = newPlant.CalculateStatus();
                await _plantService.AddPlantAsync(newPlant);
            }
            IsSaved = true;
        }

        private string _name;
        public string Name 
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _location;
        public string Location  
        {
            get => _location;
            set 
            { _location = value;
                OnPropertyChanged(nameof(Location)); 
            }
        }
        private string _notes;
        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(nameof(Notes)); }
        }
        private string _customWateringFrequencyDaysInput;
        public string CustomWateringFrequencyDaysInput
        {
            get => _customWateringFrequencyDaysInput;
            set { _customWateringFrequencyDaysInput = value; OnPropertyChanged(nameof(CustomWateringFrequencyDaysInput)); }
        }
        private Species _selectedSpecies;
        public Species SelectedSpecies
        {
            get => _selectedSpecies;
            set
            {
                _selectedSpecies = value; OnPropertyChanged(nameof(SelectedSpecies));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //el propertyname es el evento mismo tipo los subscriptores q estan esperando q pase algo
        //el invoke es para avisarle a los subs q paso algo y avisandoles q propiedad es q cambio
        //el ?. es para q no tire error si no hay subscriptores, es tipo un if
        //el new PropertyblablaArgs es el pack de info q le mando a los subs para avisarles q prop cambio
        //x eso los sets le pasan nameof(name), nameof(location) para q el wpf sepa q control refrescar 
        //en vez de refrescar todo a lo bruto
    }
}
