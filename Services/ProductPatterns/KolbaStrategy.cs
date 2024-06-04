using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class KolbaStrategy : IDistributorStrategy
    {
        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var buttonText = FindButtonWithText(web, baseUrl);
            var maxNumber = double.Parse(FindMaxNumberInButtonText(buttonText));
            var numberOfElementsPerPage = 36;

            if (maxNumber > numberOfElementsPerPage)
            {
                maxNumber += numberOfElementsPerPage;
            }

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
            var web = new HtmlWeb();
            var pageUrl = UpdatePageNumberInLink(baseUrl, currentPage);
            var document = web.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product__name");
            int i = 0;
            foreach (var productNode in productNodes)
            {

                var newProduct = new Product();
                newProduct.IdProduct = i++.ToString();
                newProduct.Name = "";
                newProduct.Price = 20;

                products.Add(newProduct);

            }

            return products;
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

        public string FindButtonWithText(HtmlWeb web, string baseUrl)
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

        public string FindMaxNumberInButtonText(string buttonText)
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
    }

}
