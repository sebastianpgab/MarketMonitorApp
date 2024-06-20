using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductPatterns;
using MarketMonitorApp.Services.ProductsStrategy;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp
{
    public interface IPriceScraper
    {
        public IEnumerable<Product> GetProducts(string baseUrl, Distributor distributor);
    }
    public class PriceScraper : IPriceScraper
    {
        private string BaseUrl;
        private readonly IDistributorStrategySelector _distributorStrategySelector;
        private readonly HtmlWeb _htmlWeb;

        public PriceScraper(IDistributorStrategySelector distributorStrategySelector, HtmlWeb htmlWeb)
        {
            _distributorStrategySelector = distributorStrategySelector;
            _htmlWeb = htmlWeb;
        }

        public IEnumerable<Product> GetProducts(string baseUrl, Distributor distributor)
        {
            BaseUrl = baseUrl;
            var allProducts = new List<Product>();
            int currentPage = 1;
            var strategy = _distributorStrategySelector.ChoseStrategy(distributor);
            int lastPage = strategy.GetLastPageNumber(_htmlWeb, baseUrl);

            while (currentPage <= lastPage)
            {
                var products = strategy.GetProducts(baseUrl, currentPage);
                allProducts.AddRange(products);

                currentPage++;
            }

            return RemoveDuplications(allProducts);
        }

        private List<Product> RemoveDuplications(List<Product> products)
        {
            return products.GroupBy(p => p.IdProduct)
                               .Select(g => g.First())
                               .ToList();
        }


    }
}
