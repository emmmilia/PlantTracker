using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantTracker.Core;

namespace PlantTracker.Data
{
    public class PlantService
    {
        private readonly PlantTrackerDBContext _context;

        public PlantService(PlantTrackerDBContext context)
        {
            _context = context;
        }

        public async Task WaterPlantAsync(int plantId, string notes)
        {
            var plant = await GetPlantByIdAsync(plantId);
            if (plant == null) return;
            plant.LastWatered = DateTime.Now;
            plant.PlantStatus = plant.CalculateStatus();
            plant.StatusLastUpdated = DateTime.Now;
            var wateringLog = new WateringLog
            {
                PlantId = plantId,
                LastWatered = DateTime.Now,
                Notes = notes
            };
            _context.WateringLogs.Add(wateringLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WateringLog>> GetWateringLogsByPlantIdAsync(int plantId)
        {
            return await _context.WateringLogs.Where(w => w.PlantId == plantId).ToListAsync();
        }
        public async Task<List<Species>> GetAllSpeciesAsync()
        {
            return await _context.Species.ToListAsync();
        }

        public async Task<List<Plant>> GetAllPlantsAsync()
        {
            return await _context.Plants.Include(p => p.Species).ToListAsync();
        }

        public async Task<Plant> GetPlantByIdAsync(int id)
        {
            return await _context.Plants.Include(p => p.Species).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPlantAsync(Plant plant)
        {
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlantAsync(Plant plant)
        {
            _context.Plants.Update(plant);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePlantAsync(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant != null)
            {
                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
