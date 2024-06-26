﻿using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class RParmsStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public RParmsStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".pagination li a")
                                                        .Where(a => int.TryParse(a.InnerText.Trim(), out _))
                                                        .ToList();
            if (paginationLinks.Count == 0)
            {
                return 1;
            }

            var lastPageString = paginationLinks[paginationLinks.Count - 1].InnerText.Trim();

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
            var productNodes = document.DocumentNode.QuerySelectorAll(".item-product");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector(".product-miniature").GetAttributeValue("data-id-product", string.Empty);
                var productName= productNode.QuerySelector(".product_name").InnerText.Trim();
                var productPrice = productNode.QuerySelector(".price").InnerText.Trim();

                decimal newPrice;
                string cleanPrice = Regex.Replace(productPrice, @"\s+|zł", "").Replace(",", ".");
                bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productName;
                newProduct.Price = newPrice;

                if (!products.Any(p => p.IdProduct == newProduct.IdProduct))
                {
                    products.Add(newProduct);
                }
            }

            return products;
        }
    }
}
