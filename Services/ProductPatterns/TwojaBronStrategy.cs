using MarketMonitorApp.Entities;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductsStrategy
{
    public class TwojaBronStrategy : IDistributorStrategy
    {

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var web = new HtmlWeb();
            var pageUrl = $"{baseUrl}/{currentPage}";
            var document = web.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.GetAttributeValue("data-product-id", string.Empty);
                var productNameNode = productNode.QuerySelector(".prodname");
                var priceElement = productNode.QuerySelector(".price em");

                var productName = productNameNode.InnerText.Trim();
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

        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
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
