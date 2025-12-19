using Microsoft.EntityFrameworkCore;
using Backend.Asmoner.Models;

namespace Backend.Asmoner.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MetadataWork> MetadataWorks { get; set; }
        public DbSet<WorkSyncInfo> WorkSyncInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "asmroner.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MetadataWork>().HasKey(m => m.ID);
            modelBuilder.Entity<WorkSyncInfo>().HasKey(w => w.ID);
            modelBuilder.Entity<MetadataWork>().HasIndex(m => m.SourceID).IsUnique();
        }
    }
}
