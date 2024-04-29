using MarketMonitorApp.Entities;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductsStrategy
{
    public class TwojaBronStrategy : IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage, HtmlDocument document)
        {
            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product");

            foreach( var productNode in productNodes )
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
    }
}
