using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantTracker.Data
{
    public class DBSeeder
    {
        public static void Seed(PlantTrackerDBContext context)
        {
            if (!context.Species.Any())
            {
                var speciesList = new List<Core.Species>
                {
                    new Core.Species { Id = 1, Name = "Cacti/Succulents", Description = "Stores water, tolerates drought", wateringFrequency = 21 },
                    new Core.Species { Id= 2, Name = "Large-leaf tropical", Description = "E.g. Monstera, ficus", wateringFrequency = 10 },
                    new Core.Species { Id= 3, Name = "Herbs", Description = "Rosemary, lavender", wateringFrequency = 10 },
                    new Core.Species { Id= 4, Name = "Constant Moisture", Description = "E.g. Peace Lily, Fern", wateringFrequency = 5 },
                    new Core.Species { Id = 5, Name = "Vegetable/Garden", Description = "E.g. Arugula, basil", wateringFrequency = 4 }
                };

                context.Species.AddRange(speciesList);
                context.SaveChanges();
            }
        }
    }
}
