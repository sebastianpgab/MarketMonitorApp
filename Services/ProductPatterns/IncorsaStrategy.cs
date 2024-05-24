using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class IncorsaStrategy : IDistributorStrategy
    {
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
            var pageUrl = $"{baseUrl}";
            var document = web.Load(pageUrl);

            var products = new List<Product>();
            //połącz wyniki
            var imgDetailsNodes = document.DocumentNode.QuerySelectorAll(".abs-layout-img-and-details");
            var purchaseDetailsNodes = document.DocumentNode.QuerySelectorAll(".abs-layout-purchase");

            var productNodes = document.DocumentNode.QuerySelectorAll(".abs-layout-product-list");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".abs-p-catalog-index > span:nth-child(2)");
                var productNameNode = productNode.QuerySelector(".abs-product-name a");
                var priceElement = productNode.QuerySelector(".abs-item-price-amount");

                var price = priceElement.InnerText.Trim();

                decimal newPrice;
                string cleanPrice = Regex.Replace(price, @"\s+|zł", "").Replace(",", ".");
                bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                var newProduct = new Product();
                newProduct.IdProduct = productId.InnerText.Trim();
                newProduct.Name = productNameNode.InnerText.Trim();
                newProduct.Price = newPrice;

                products.Add(newProduct);

            }

            return products;
        }
    }
}
