using MarketMonitorApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MarketMonitorApp.Services
{
    public interface IDistributorDetailsService
    {
        Distributor GetDistributorByName(string name);
        void AddActualization(Distributor distributor, string categoryName, DateTime date); // Przykładowa metoda dodawania aktualizacji
    }

    public class DistributorDetailsService : IDistributorDetailsService
    {
        private readonly MarketMonitorDbContext _context;

        public DistributorDetailsService(MarketMonitorDbContext context)
        {
            _context = context;
        }

        public Distributor GetDistributorByName(string name)
        {
           var distributor = _context.Distributors
                                          .Include(d => d.Categories)
                                          .FirstOrDefault(d => d.Name == name);
            if (distributor == null)
            {
                throw new InvalidOperationException("Dystrybutor o tej nazwie nie został znaleziony.");
            }

            return distributor;         
        }

        public void AddActualization(Distributor distributor, string categoryName, DateTime date)
        {
            
        }
    }
}
