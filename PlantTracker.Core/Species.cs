using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantTracker.Core
{
    public class Species
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int wateringFrequency { get; set; }

        public Species(int Id, string Name, string description, int wateringFrequency) {
            this.Id = Id;
            this.Name = Name;
            this.Description = description;
            this.wateringFrequency = wateringFrequency;
        }
    }
}
