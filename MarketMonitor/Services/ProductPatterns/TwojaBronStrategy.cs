using MarketMonitorApp.Entities;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Globalization;
using System.Text.RegularExpressions;
using MarketMonitorApp.Helpers;

namespace MarketMonitorApp.Services.ProductsStrategy
{
    public class TwojaBronStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public TwojaBronStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var pageUrl = $"{baseUrl}/{currentPage}";
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.GetAttributeValue("data-product-id", string.Empty);
                var productName = productNode.QuerySelector(".prodname").InnerText.Trim();
                ValidationHelper.ValidateProductName(productName);
                var priceElement = productNode.QuerySelector(".price em");

                var price = priceElement == null ? "0" : priceElement.InnerText.Trim();

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productName;
                newProduct.Price = CleanPrice(price);

                if (!products.Any(p => p.IdProduct == newProduct.IdProduct))
                {
                    products.Add(newProduct);
                }
            }

            return products;
        }

        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
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

        public decimal CleanPrice(string price)
        {
            if (string.IsNullOrWhiteSpace(price))
            {
                return 0;
            }

            string cleanPrice = Regex.Replace(price, @"\s+|zł", "").Replace(",", ".");

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
