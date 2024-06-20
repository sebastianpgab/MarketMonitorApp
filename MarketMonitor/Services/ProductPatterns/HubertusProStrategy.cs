using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class HubertusProStrategy : IDistributorStrategy
    {
        private readonly HtmlWeb _htmlWeb;
        public HubertusProStrategy(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var pageUrl = $"{baseUrl}";
            if(currentPage != 1)
            {
              pageUrl = $"{baseUrl}{currentPage}";
            }
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".image a").GetAttributeValue("href", string.Empty);
                var productNameNode = productNode.QuerySelector(".product_name");
                var priceElement = productNode.QuerySelector(".price");

                var productName = productNameNode.InnerText.Trim();
                var price = priceElement.InnerText.Trim();

                decimal newPrice;
                string cleanPrice = Regex.Replace(price, @"[^\d,]", "").Replace(",", ".");
                bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                var newProduct = new Product();
                newProduct.IdProduct = CutUrl(productId);
                newProduct.Name = productName;
                newProduct.Price = newPrice;

                products.Add(newProduct);
            }

            return products;
        }

        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.QuerySelectorAll(".pagination li a");

            if (paginationLinks == null || !paginationLinks.Any())
            {
                return 1;
            }

            var hrefValue = paginationLinks.LastOrDefault()?.GetAttributeValue("href", string.Empty);
            var lastSegment = hrefValue?.Split('/').LastOrDefault();

            return int.TryParse(lastSegment, out int lastPageNumber) ? lastPageNumber : 1;
        }

        private string CutUrl(string url)
        {
            string pattern = @"manufacturer/([^\.]+)\.html";
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
