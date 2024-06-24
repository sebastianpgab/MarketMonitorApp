using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class KniejaStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;
        public KniejaStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var pageUrl = $"{baseUrl}{currentPage}";
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product-miniature");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.GetAttributeValue("data-id-product", string.Empty);
                var productNameNode = productNode.QuerySelector(".product-title");
                var priceElement = productNode.QuerySelector(".price");

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
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".page-item a.page-link")
                                                         .Select(a => a.InnerText.Trim())
                                                         .Where(text => int.TryParse(text, out _))
                                                         .Select(int.Parse)
                                                         .ToList();

            var lastPageNumber = paginationLinks.Any() ? paginationLinks.Max() : 1;

            return lastPageNumber;
        }
    }
}
