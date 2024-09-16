﻿using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Helpers;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class KaliberStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public KaliberStrategy(IHtmlWebAdapter htmlWeb)
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
                var productName = productNode.QuerySelector(".product-name").InnerText.Trim();
                ValidationHelper.ValidateProductName(productName);
                var priceElement = productNode.QuerySelector(".price");

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
    }
}
