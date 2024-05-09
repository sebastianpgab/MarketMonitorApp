using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductPatterns;
using MarketMonitorApp.Services.ProductsStrategy;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        public PriceScraper(IDistributorStrategySelector distributorStrategySelector)
        {
            _distributorStrategySelector = distributorStrategySelector;
        }

        public IEnumerable<Product> GetProducts(string baseUrl, Distributor distributor)
        {
            BaseUrl = baseUrl;
            var allProducts = new List<Product>();
            HtmlWeb web = new HtmlWeb();
            int currentPage = 1;
            var strategy = _distributorStrategySelector.ChoseStrategy(distributor);
            int lastPage = strategy.GetLastPageNumber(web, baseUrl);

            while (currentPage <= lastPage)
            {
                var products = strategy.GetProducts(baseUrl, currentPage);
                allProducts.AddRange(products);

                currentPage++;
            }

            return allProducts;
        }


    }
}
