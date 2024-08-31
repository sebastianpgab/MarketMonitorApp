using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using MarketMonitorApp.Helpers;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class DeltaOpticalStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public DeltaOpticalStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var url = baseUrl + currentPage;
            var documentHtml = _htmlWeb.Load(baseUrl);
            var prodcuts = documentHtml.DocumentNode.QuerySelectorAll(".product-box");
            var products = new List<Product>();

            foreach ( var prodcut in prodcuts)
            {
                var productId = prodcut.GetAttributeValue("data-product-id", string.Empty);
                var productName = prodcut.QuerySelector(".product-name").InnerText.Trim();
                ValidationHelper.ValidateProductName(productName);
                var priceElement = prodcut.QuerySelector(".price");

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

        public int GetLastPageNumber(IHtmlWebAdapter webAdapter, string url)
        {
            var htmlDocument = webAdapter.Load(url);
            var paginatorInfoText = htmlDocument.DocumentNode.QuerySelector(".paginator-info").InnerText;

            if (!string.IsNullOrEmpty(paginatorInfoText))
            {
                bool isParsedSuccessfully = int.TryParse(paginatorInfoText.Replace("(", "").Replace(")", "").Trim(), out int totalProductsCount);
                if (isParsedSuccessfully)
                {
                    double totalPages = Math.Ceiling(totalProductsCount / 12.0);
                    return (int)totalPages;
                }
            }
            return 1;
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
