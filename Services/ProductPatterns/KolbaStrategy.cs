using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Reflection.Metadata;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class KolbaStrategy : IDistributorStrategy
    {
        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var buttonText = FindButtonWithText(web, baseUrl);
            var maxNumber = FindMaxNumberInButtonText(buttonText);

            return 1;
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            throw new NotImplementedException();
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
            return null;
        }
    }

}
