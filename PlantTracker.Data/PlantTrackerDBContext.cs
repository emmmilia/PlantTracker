using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantTracker.Core;

namespace PlantTracker.Data
{
    public class PlantTrackerDBContext : DbContext
    {
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<WateringLog> WateringLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "planttracker.db");
            optionsBuilder.UseSqlite($"Data Source = {dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plant>()
                .HasOne(p => p.Species)
                .WithMany()
                .HasForeignKey(p => p.SpeciesId);
            modelBuilder.Entity<Plant>()
                .HasMany<WateringLog>()
                .WithOne()
                .HasForeignKey(w => w.PlantId);
        }
    }
}
