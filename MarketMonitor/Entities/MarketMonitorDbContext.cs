using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MarketMonitorApp.Entities
{
    public class MarketMonitorDbContext : DbContext
    {
        private string _connectionString = "Server=TB-PC17\\SQLEXPRESS; Database=Market; User ID=sa; Password=Twojabron123!";
        public DbSet<Actualization> Actualizations { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actualization>()
                .HasMany(a => a.Products)
                .WithOne(p => p.Actualization)
                .HasForeignKey(p => p.ActualizationId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
