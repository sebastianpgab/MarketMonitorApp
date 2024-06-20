using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class MalikMalikStrategy : IDistributorStrategy
    {
        private readonly HtmlWeb _htmlWeb;

        public MalikMalikStrategy(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".pagination li a")
                                                        .Where(a => int.TryParse(a.InnerText.Trim(), out _))
                                                        .ToList();
            if (paginationLinks.Count == 0)
            {
                return 1;
            }

            var lastPageString = paginationLinks[paginationLinks.Count - 1].InnerText.Trim();

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
            var pageUrl = $"{baseUrl}{currentPage}";
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".djc_item_in");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".djc_price").GetAttributeValue("data-itemid", string.Empty);
                var productName = productNode.QuerySelector(".djc_title").InnerText.Trim();
                var priceNode = productNode.QuerySelector(".djc_price_value");
                var price = priceNode != null ? priceNode.InnerText.Trim() : "0";

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
    }
}
