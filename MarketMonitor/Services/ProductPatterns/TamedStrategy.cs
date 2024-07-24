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
        private readonly IHtmlWebAdapter _htmlWeb;

        public TamedStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
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

        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.QuerySelectorAll(".page-list li a").ToList();

            if (!paginationLinks.Any())
            {
                return 1;
            }

            var lastPageString = paginationLinks[^2].InnerText.Trim();

            return int.TryParse(lastPageString, out int lastPageNumber) ? lastPageNumber : 1;
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var pageUrl = $"{baseUrl}{currentPage}";
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".ajax_block_product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".leo-more-info").GetAttributeValue("data-idproduct", string.Empty);
                var productNameNode = productNode.QuerySelector(".product-title a").GetAttributeValue("href", string.Empty);
                var productName = ExtractProductNameFromLink(productNameNode);
                var priceElement = productNode.QuerySelector(".price span[itemprop='price']");

                var price = priceElement == null ? "0" : priceElement.InnerText.Trim();

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productName;
                newProduct.Price = CleanPrice(price);

                products.Add(newProduct);
            }
            return products;
        }

        private string ExtractProductNameFromLink(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return string.Empty;
            }

            var linkSegments = link.Split('/');
            return linkSegments.Length > 0 ? linkSegments[^1].Replace("-", " ").Trim() : string.Empty;
        }
    }
}
