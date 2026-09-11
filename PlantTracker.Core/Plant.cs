using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantTracker.Core
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Species Species { get; set; }
        public DateTime LastWatered { get; set; }
        public int? CustomWateringFrequencyDays { get; set; }
        public PlantStatus PlantStatus { get; set; }
        public DateTime StatusLastUpdated { get; set; }
        public DateTime DateAdded { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }

        public Plant(int id, string name, Species species, DateTime lastWatered, int? customWateringFrequencyDays, PlantStatus plantStatus, DateTime statusLastUpdated, DateTime dateAdded, string? location, string? notes)
        {
            this.Id = id;
            this.Name = name;
            this.Species = species;
            this.LastWatered = lastWatered;
            this.CustomWateringFrequencyDays = customWateringFrequencyDays;
            this.PlantStatus = plantStatus;
            this.StatusLastUpdated = statusLastUpdated;
            this.DateAdded = dateAdded;
            this.Location = location;
            this.Notes = notes;
        }

        public int GetWateringFrequencyDays() {
            if (CustomWateringFrequencyDays != null) { 
                return CustomWateringFrequencyDays.Value;
            }
            else
            {
                return Species.wateringFrequency;
            }
        }

        public PlantStatus CalculateStatus() {
            double daysPassed = (DateTime.Today - LastWatered).Days;
            double frequency = GetWateringFrequencyDays();
            double ratio = daysPassed / frequency;

            if (ratio <= 1.0) return PlantStatus.Healthy;
            else if (ratio <= 1.3) return PlantStatus.ThirstySoon;
            else if (ratio <= 2.0) return PlantStatus.Thirsty;
            else if (ratio <= 3.0) return PlantStatus.Critical;
            else return PlantStatus.Dead;
        }
    }
}
