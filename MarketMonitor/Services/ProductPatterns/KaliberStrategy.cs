using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class KaliberStrategy : IDistributorStrategy
    {
        private readonly HtmlWeb _htmlWeb;

        public KaliberStrategy(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.QuerySelectorAll(".pagination li a").ToList();

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
            var pageUrl = $"{baseUrl}{currentPage}";
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".ajax_block_product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".add_to_compare").GetAttributeValue("data-id-product", string.Empty);
                var productNameNode = productNode.QuerySelector(".product-name");
                var priceElement = productNode.QuerySelector(".price");

                var productName = productNameNode.InnerText.Trim();
                var price = priceElement != null ? priceElement.InnerText.Trim() : "0";

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
