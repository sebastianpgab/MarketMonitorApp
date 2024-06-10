using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class TamedStrategy : IDistributorStrategy
    {
        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.QuerySelectorAll(".page-list li a").ToList();

            if (paginationLinks.Count == 0)
            {
                return 1;
            }

            var lastPageString = paginationLinks[paginationLinks.Count - 2].InnerText.Trim();

            if (int.TryParse(lastPageString, out int lastPageNumber))
            {
                return lastPageNumber;
            }
            else
            {
                return 1;
            }
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var web = new HtmlWeb();
            var pageUrl = $"{baseUrl}{currentPage}";
            var document = web.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".ajax_block_product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".leo-more-info").GetAttributeValue("data-idproduct", string.Empty);
                var productNameNode = productNode.QuerySelector(".product-title a").GetAttributeValue("href", string.Empty);
                var productName = TakeProductName(productNameNode);
                var priceElement = productNode.QuerySelector(".price em");

                var price = priceElement.InnerText.Trim();

                decimal newPrice;
                string cleanPrice = Regex.Replace(price, @"\s+|zł", "").Replace(",", ".");
                bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productName;
                newProduct.Price = newPrice;

                products.Add(newProduct);
            }
            return products;
        }

        private string TakeProductName(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                var linkElements = link.Split('/');
                if (linkElements.Length > 0)
                {
                    var lastElement = linkElements[^1].Replace("-", " ").Trim();
                    return lastElement;
                }
            }
            return null;
        }
    }
}
