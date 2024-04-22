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
        void CompareProducts(Actualization actualization, int distributorId);
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

        public void CompareProducts(Actualization actualization, int distributorId)
        {
            //wez produkty gdzie dystrybutor zgadza się z id 
            //gdzie categoria zgadza się
            //gdzie jest ostatnia aktulizacja
            var idLastActualization = _context.Actualizations.Max(p => p.Id);
            List<Actualization> sortedByDistributorAndActulization = null;
            if (idLastActualization >= 1)
            {

                sortedByDistributorAndActulization = _context.Actualizations
               .Include(p => p.Products)
               .Where(p => p.DistributorId == actualization.DistributorId && p.Id == idLastActualization)
               .ToList();

                //id kategorii, ktora chcemy zaktalizować 
                int IdCategory = actualization.Distributor.Categories.Select(p => p.Id).First();
                //lista kategorii no i co dalej ?
                var listOfCategories = sortedByDistributorAndActulization.Select(p => p.Distributor).SelectMany(p => p.Categories).Where(p => p.Id == IdCategory).ToList();
            }

            var products = sortedByDistributorAndActulization.SelectMany(p => p.Products).ToList() ;




        }
    }
}
