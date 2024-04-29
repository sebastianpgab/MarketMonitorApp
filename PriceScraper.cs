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
            int lastPage = GetLastPageNumber(web);

            while (currentPage <= lastPage)
            {
                web = new HtmlWeb();
                var pageUrl = $"{BaseUrl}/{currentPage}";
                var document = web.Load(pageUrl);

                var strategy = _distributorStrategySelector.ChoseStrategy(distributor);

                var products = strategy.GetProducts(baseUrl, currentPage, document);
                allProducts.AddRange(products);

                currentPage++;
            }

            return allProducts;
        }

        private int GetLastPageNumber(HtmlWeb web)
        {
            var document = web.Load(BaseUrl);
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".paginator li a")
                                                        .Where(a => int.TryParse(a.InnerText.Trim(), out _))
                                                        .ToList();

            var lastPageNumber = 1; 

            if (paginationLinks.Any())
            {
                if (int.TryParse(paginationLinks.Last().InnerText.Trim(), out int pageNumber))
                {
                    lastPageNumber = pageNumber;
                }
            }

            return lastPageNumber;
        }
    }
}
