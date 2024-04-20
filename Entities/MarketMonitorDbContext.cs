using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MarketMonitorApp.Entities
{
    public class MarketMonitorDbContext : DbContext 
    {
        private string _connectionString = "Server=SEBASTIANPGAB\\SQLEXPRESS; Database=MarketMonitorDb; Trusted_Connection=True";
        public DbSet<Actualization> Actualizations { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
