using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Helpers;
using MarketMonitorApp.Services.ProductsStrategy;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class KolbaStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public KolbaStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
        {
            var buttonText = FindButtonWithText(web, baseUrl);
            var maxNumber = buttonText == null || buttonText == string.Empty ? 0 : double.Parse(FindMaxNumberInButtonText(buttonText));
            var numberOfElementsPerPage = 36;

            maxNumber += numberOfElementsPerPage;

            double result = maxNumber / numberOfElementsPerPage;

            if (result % 1 > 0)
            {
                int pages = (int)Math.Ceiling(result);
                return pages;
            }

            return (int)result;
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var pageUrl = UpdatePageNumberInLink(baseUrl, currentPage);
            var document = _htmlWeb.Load(pageUrl);

            var products = new List<Product>();

            var productName = document.DocumentNode.QuerySelectorAll(".product__name");
            var productPrice = document.DocumentNode.QuerySelectorAll(".product__price");

            var productsNode = productName.Zip(productPrice, (prodName, prodPrice) => new
            { 
              ProdName = prodName, 
              ProdPrice = prodPrice 
            });

            foreach (var productNode in productsNode)
            {
                var linkHtml = productNode.ProdName.QuerySelector("a").GetAttributeValue("href", string.Empty);
                var productId = GetProductId(linkHtml);
                var productNameNode = productNode.ProdName.InnerText.Trim();
                ValidationHelper.ValidateProductName(productNameNode);
                var priceElement = productNode.ProdPrice.QuerySelector(".flex .flex .text-xl");

                var price = priceElement != null ? priceElement.InnerText.Trim() : "0";

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productNameNode;
                newProduct.Price = CleanPrice(price);

                if (!products.Any(p => p.IdProduct == newProduct.IdProduct))
                {
                    products.Add(newProduct);
                }
            }
            return products;
        }

        private string GetProductId(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return null;
            }

            var lastSlashIndex = link.LastIndexOf('/');
            var commaIndex = link.IndexOf(',');

            if (lastSlashIndex == -1 || commaIndex == -1 || commaIndex <= lastSlashIndex)
            {
                return null;
            }

            return link.Substring(lastSlashIndex + 1, commaIndex - lastSlashIndex - 1);
        }

        private string UpdatePageNumberInLink(string url, int currentPage)
        {
            int queryStartIndex = url.IndexOf('?');
            if (queryStartIndex == -1)
            {
                return url;
            }

            string baseUrl = url.Substring(0, queryStartIndex);
            string query = url.Substring(queryStartIndex + 1);
            string[] parameters = query.Split('&');

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].StartsWith("page="))
                {
                    parameters[i] = "page=" + currentPage;
                    break;
                }
            }

            string newQuery = string.Join("&", parameters);

            return baseUrl + "?" + newQuery;
        }

        private string FindButtonWithText(IHtmlWebAdapter web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var buttons = document.DocumentNode.SelectNodes("//button");

            if (buttons != null)
            {
                var targetButton = buttons
                    .Where(button => button.InnerText.Contains("Załaduj następne"))
                    .FirstOrDefault();

                if (targetButton != null)
                {
                    return targetButton.InnerText;
                }
            }
            return null;
        }

        private string FindMaxNumberInButtonText(string buttonText)
        {
            if(buttonText != null)
            {
                int openParenIndex = buttonText.IndexOf("(");
                int closeParenIndex = buttonText.IndexOf(")");

                if (openParenIndex != -1 && closeParenIndex != -1 && closeParenIndex > openParenIndex)
                {
                    int substringLength = closeParenIndex - openParenIndex - 1;
                    string numbersSubstring = buttonText.Substring(openParenIndex + 1, substringLength).ToLower();

                    var numberParts = numbersSubstring.Split('z');

                    var lastPart = numberParts.Last().Trim();
                    return lastPart;
                }
            }
            return "1";
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
