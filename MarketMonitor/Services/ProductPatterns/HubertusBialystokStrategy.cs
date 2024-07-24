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
        private readonly IHtmlWebAdapter _htmlWeb;

        public HubertusBialystokStrategy(IHtmlWebAdapter htmlWeb)
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
                var productId = productNode.QuerySelector(".abs-catalog-index").InnerText.Trim();
                var productName = productNode.QuerySelector(".abs-product-name").InnerText.Trim();
                var priceElement = productNode.QuerySelector(".abs-item-price-amount");

                var price = priceElement == null ? "0" : priceElement.InnerText.Trim();

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productName;
                newProduct.Price = CleanPrice(price);

                products.Add(newProduct);
            }
            return products;
        }

        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
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

        public decimal CleanPrice(string price)
        {
            if (string.IsNullOrWhiteSpace(price))
            {
                return 0;
            }

            var cleanPrice = price
                .Replace("zł", "")
                .Replace("brutto", "")
                .Replace(" ", "")
                .Replace("\u00A0", "")
                .Replace(",", ".")
                .Trim();

            if (decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal newPrice))
            {
                return newPrice;
            }
            else
            {
                throw new FormatException($"The price '{price}' is not in a valid format.");
            }
        }
    }
}
