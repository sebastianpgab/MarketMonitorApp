using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class SzusterStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public SzusterStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var pageUrl = currentPage == 1 ? baseUrl : $"{baseUrl}/{currentPage}";
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product");

            foreach (var productNode in productNodes)
            {
                var product_page_url = productNode.QuerySelector(".name a").GetAttributeValue("href", string.Empty);
                var productId = Regex.Replace(product_page_url, "^/manufacturer/|\\.html$", "");
                var productName = ExtractProductNameFromLink(product_page_url);
                var priceElement = productNode.QuerySelector(".price");

                var price = priceElement == null ? "0" : priceElement.InnerText.Trim();

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productName;
                newProduct.Price = CleanPrice(price);

                products.Add(newProduct);
            }

            return products;
        }

        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLink = document.DocumentNode.QuerySelectorAll(".pagination li a").LastOrDefault();

            if (paginationLink != null)
            {
                var paginationLinkHref = paginationLink.GetAttributeValue("href", string.Empty);
                int lastPageNumber = ExtractPageNumberFromHref(paginationLinkHref);

                return lastPageNumber;
            }

            return 1; // Brak paginacji, więc domyślnie 1
        }

        private int ExtractPageNumberFromHref(string href)
        {
            var parts = href.Split('/');
            var lastPart = parts.Last();

            if (int.TryParse(lastPart, out int pageNumber))
            {
                return pageNumber;
            }

            return 1;
        }

        private string ExtractProductNameFromLink(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return string.Empty;
            }
            var linkSegments = link.Split('/');
            return linkSegments.Length > 0 ? linkSegments[^1].Replace("-", " ").Replace(".html", "").Trim() : string.Empty;
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
