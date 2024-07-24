using CsvHelper;
using MarketMonitorApp.Entities;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services
{
    public interface IDistributorDetailsService
    {
        Distributor GetDistributorByName(string name);
        Actualization AddActualization(List<Product> products, Distributor distributor, Category category);
        List<Product> CompareProducts(Actualization actualization, Category category);
        bool ExportProductsToCsv(List<Product> comparedProdcuts, Actualization actualization, string categoryName);
        public List<Product> LastUpdatedProducts(Actualization actualization, Category category);
        public Category GetCategoryByLink(string link);

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
                return null;
            }

            return distributor;
        }

        public Category GetCategoryByLink(string link)
        {
            var category = _context.Categories.FirstOrDefault(p => p.LinkToCategory == link);
            if (category == null)
            {
                return null;
            }

            return category;
        }

        public Actualization AddActualization(List<Product> products, Distributor distributor, Category category)
        {
            var newActualization = new Actualization();
            newActualization.Products = products;
            newActualization.DistributorId = distributor.Id;
            newActualization.Distributor = distributor;
            newActualization.CategoryId = category.Id;

            _context.Actualizations.Add(newActualization);
            _context.SaveChanges();
            return newActualization;
        }

        public List<Product> CompareProducts(Actualization actualization, Category category)
        {
            var productsTab = LastUpdatedProducts(actualization, category);
            if (productsTab == null) return null;

            var latestUpdatedProducts = actualization.Products.ToList();
            List<Product> comparedProducts = new List<Product>();


            var productsTabIds = new HashSet<string>(productsTab.Select(p => p.IdProduct));

            foreach (var product in latestUpdatedProducts)
            {

                if (!productsTabIds.Contains(product.IdProduct))
                {
                    product.IsNew = true;
                    comparedProducts.Add(product);
                }
                else
                {

                    var originalProduct = productsTab.FirstOrDefault(p => p.IdProduct == product.IdProduct);
                    if (originalProduct != null && originalProduct.Price != product.Price)
                    {
                        comparedProducts.Add(product);
                    }
                }
            }

            return comparedProducts;
        }



        public bool ExportProductsToCsv(List<Product> comparedProducts, Actualization actualization, string categoryName)
        {
            if (comparedProducts == null || comparedProducts.Count == 0)
            {
                return false;
            }

            var distributorName = actualization.Distributor.Name;


            string filePath = @"C:\Users\damia\Desktop\xx";

            DateTime currentData = DateTime.Now;

            string currentDataConverted = currentData.ToString("dd MM yyyy__HH-mm-ss");
            string fileName = $"{distributorName}-{categoryName}-{currentDataConverted}-products.csv";
            string fullPath = Path.Combine(filePath, fileName);

            using (var writer = new StreamWriter(fullPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(comparedProducts);
            }
            return true;
        }

        public List<Product> LastUpdatedProducts(Actualization actualization, Category category)
        {
            // Znajdź ostatnią aktualizację
            var actualizations = _context.Actualizations.Max(p => p.Id);

            if (actualizations >= 1)
            {
                // Pobierz ostatnią aktualizację z odpowiednim dystrybutorem i kategorią
                var lastActualization = _context.Actualizations
                    .Include(p => p.Products)  // Uwzględnij powiązane produkty
                    .Where(p => p.DistributorId == actualization.DistributorId && p.CategoryId == category.Id)
                    .OrderByDescending(p => p.Id)
                    .Skip(1) // Pomija ostatni dodany element, czyli bierze przedostatni
                    .FirstOrDefault();

                if (lastActualization != null)
                {
                    // Pobierz produkty powiązane z tą aktualizacją
                    return lastActualization.Products.ToList();
                }
            }

            return new List<Product>(); // Zwraca pustą listę zamiast `null`
        }
    }
}
