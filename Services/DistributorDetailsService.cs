using MarketMonitorApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MarketMonitorApp.Services
{
    public interface IDistributorDetailsService
    {
        Distributor GetDistributorByName(string name);
        Actualization AddActualization(List<Product> products, Distributor distributor);
        void CheckDiferents(Actualization actualization);
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

        public Actualization AddActualization(List<Product> products, Distributor distributor)
        {
            var newActualization = new Actualization();
            newActualization.Products = products;
            newActualization.DistributorId = distributor.Id;
            newActualization.Distributor = distributor;

            _context.Actualizations.Add(newActualization);
            _context.SaveChanges();
            return newActualization;
        }

        public void CheckDiferents(Actualization actualization)
        {

        }
    }
}
