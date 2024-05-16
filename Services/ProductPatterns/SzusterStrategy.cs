using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class SzusterStrategy : IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var web = new HtmlWeb();
            var pageUrl = currentPage == 1 ? baseUrl : $"{baseUrl}/{currentPage}";
            var document = web.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".image a").GetAttributeValue("href", string.Empty);
                var productName = productNode.QuerySelector(".name");
                var priceValue = productNode.QuerySelector(".price");

                var productNameValue = productName.InnerText.Trim();
                var price = priceValue.InnerText.Trim();

                decimal newPrice;
                string cleanPrice = Regex.Replace(price, @"\s+|zł", "").Replace(",", ".");
                bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                var newProduct = new Product();
                newProduct.IdProduct = CutUrl(productId);
                newProduct.Name = productNameValue;
                newProduct.Price = newPrice;

                products.Add(newProduct);

            }

            return products;
        }

        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLink = document.DocumentNode.QuerySelectorAll(".pagination li a").LastOrDefault();

            var paginationLinkHref = paginationLink.GetAttributeValue("href", string.Empty);

            int lastPageNumber = 1;

            if (1>0)
            {
                var parts = paginationLinkHref.Split('/');
                var lastPart = parts.Last(); 

                if (int.TryParse(lastPart, out int pageNumber))
                {
                    lastPageNumber = pageNumber;
                }
            }

            return lastPageNumber;
        }

        private string CutUrl(string url)
        {
            string pattern = @"manufacturer/(.+)\.html";
            Match match = Regex.Match(url, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
