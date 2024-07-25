using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class HubertusProStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;
        public HubertusProStrategy(IHtmlWebAdapter htmlWeb)
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
                var productName = productNode.QuerySelector(".product_name").InnerText.Trim();
                var priceElement = productNode.QuerySelector(".price");

                var price = priceElement == null ? "0" : priceElement.InnerText.Trim();

                var newProduct = new Product();
                newProduct.IdProduct = CutUrl(productId);
                newProduct.Name = productName;
                newProduct.Price = CleanPrice(price);

                products.Add(newProduct);
            }

            return products;
        }

        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
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

        public decimal CleanPrice(string price)
        {
            if (string.IsNullOrWhiteSpace(price))
            {
                return 0;
            }

            string cleanPrice = Regex.Replace(price, @"[^\d,]", "").Replace(",", ".");

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
