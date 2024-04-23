using MarketMonitorApp.Entities;
using Microsoft.AspNetCore.Mvc.TagHelpers;
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
            var idLastActualization = _context.Actualizations.Max(p => p.Id);
            List<Actualization> sortedByDistributorAndActulization = null;
            if (idLastActualization >= 1)
            {
                sortedByDistributorAndActulization = _context.Actualizations
               .Include(p => p.Products)
               .Where(p => p.DistributorId == actualization.DistributorId && p.Id == idLastActualization-1)
               .ToList();

                int idCategory = actualization.Distributor.Categories.Select(p => p.Id).First();

                var result = sortedByDistributorAndActulization
                    .Where(actualization => actualization.Distributor.Categories.FirstOrDefault(category => category.Id == idCategory) != null)
                    .ToList();

                var productsTab = sortedByDistributorAndActulization.SelectMany(p => p.Products).ToList();

                var newProducts = actualization.Products.ToList();

                List<Product> list = new List<Product>();

                foreach (var itemTab in productsTab)
                {
                    if(itemTab != null && newProducts != null)
                    {
                       var product = newProducts.Where( p => p.IdProduct == itemTab.IdProduct).FirstOrDefault();
                        if(product != null)
                        {
                            if(product.Price != itemTab.Price){
                                list.Add(product);
                            }
                        }

                    }

                    
                }
            }
            else
            {
                return ;
            }






        }
    }
}
