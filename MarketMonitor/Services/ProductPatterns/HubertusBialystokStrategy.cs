using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class HubertusBialystokStrategy : IDistributorStrategy
    {
        private readonly HtmlWeb _htmlWeb;

        public HubertusBialystokStrategy(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            string pattern = @"(?<=search/\d+,)\d+(?=,default-asc)";
            string replacement = currentPage.ToString();
            string modifiedUrl = Regex.Replace(baseUrl, pattern, replacement);
            var document = _htmlWeb.Load(modifiedUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".abs-layout-product-gallery");

            foreach (var productNode in productNodes)
            {
                var productIdNode = productNode.QuerySelector(".abs-catalog-index");
                var productNameNode = productNode.QuerySelector(".abs-product-name");
                var priceElementNode = productNode.QuerySelector(".abs-item-price-amount");

                var productName = productNameNode.InnerText.Trim();
                var cleanPrice = priceElementNode != null ? priceElementNode.InnerText
                .Replace("zł", "")
                .Replace("brutto", "")
                .Replace(" ", "")
                .Replace("\u00A0", "") 
                .Replace(",", ".")
                .Trim() : "Price not found";

                decimal newPrice;
                bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                var newProduct = new Product();
                newProduct.IdProduct = productIdNode.InnerText.Trim();
                newProduct.Name = productNameNode.InnerText.Trim();
                newProduct.Price = newPrice;

                products.Add(newProduct);
            }
            return products;
        }

        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".pagination li a")
                                                        .Where(a => int.TryParse(a.InnerText.Trim(), out _))
                                                        .ToList();
            var lastPageNumber = 1;

            if (paginationLinks.Count > 1)
            {
                string lastPageNumberString = paginationLinks[paginationLinks.Count - 1].InnerHtml; 

                lastPageNumber = int.Parse(lastPageNumberString);
            }

            return lastPageNumber;
        }
    }
}
