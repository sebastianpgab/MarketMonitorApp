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
            var pageUrl = baseUrl + currentPage; ;
            var service = ChromeDriverService.CreateDefaultService();
            var productsList = new List<Product>();
            var options = new ChromeOptions();

            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            options.AddArguments("headless");

            using (var driver = new ChromeDriver(service, options))
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    driver.Navigate().GoToUrl(pageUrl);
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                    wait.Until(driver => (string)jsExecutor.ExecuteScript("return document.readyState") == "complete");

                    // Akceptacja plików cookie za pomocą JavaScript
                    AkcceptCookieFiles(wait, driver, jsExecutor);

                    // Symulacja przewijania strony
                    var products = SimulateScroll(wait, driver, jsExecutor);

                    foreach (var productElement in products)
                    {
                        var productId = productElement.GetAttribute("data-product-id");
                        var productName = productElement.FindElement(By.CssSelector("h2.product-name > a")).GetAttribute("title").Trim();
                        var priceElement = productElement.FindElement(By.CssSelector(".price"));

                        string price = priceElement?.Text?.Trim() ?? "0";

                        if (string.IsNullOrEmpty(price) || price == "0")
                        {
                            var discountPriceElement = productElement.FindElement(By.CssSelector(".discount-price"));
                            price = discountPriceElement?.Text?.Trim() ?? "0";
                        }

                        var product = new Product
                        {
                            IdProduct = productId,
                            Name = productName,
                            Price = CleanPrice(price)
                        };

                        if (!productsList.Any(p => p.IdProduct == product.IdProduct))
                        {
                            productsList.Add(product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fail: " + ex.Message);
                }
                finally
                {
                    driver.Quit();
                }
            }

            return productsList;
        }

        private void AkcceptCookieFiles(WebDriverWait wait, ChromeDriver driver, IJavaScriptExecutor jsExecutor)
        {
            try
            {
                var acceptCookiesButton = wait.Until(driver => driver.FindElement(By.CssSelector(".btn.btn-primary[data-type='accept-cookies-button']")));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                jsExecutor.ExecuteScript("arguments[0].click();", acceptCookiesButton);
                Console.WriteLine("Cookie files was accepted");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("There was not baner with cookie files");
            }
        }

        private ReadOnlyCollection<IWebElement> SimulateScroll(WebDriverWait wait, ChromeDriver driver, IJavaScriptExecutor jsExecutor)
        {
            int numberOfItems = 0;
            int newNumberOfItems = 0;
            ReadOnlyCollection<IWebElement> products = null;
            const int maxScrollAttempts = 2; // Liczba maksymalnych prób przewijania
            int scrollAttempts = 0;

            while (scrollAttempts < maxScrollAttempts)
            {
                products = driver.FindElements(By.CssSelector(".product-box"));
                newNumberOfItems = products.Count;

                if (newNumberOfItems == numberOfItems)
                {
                    scrollAttempts++;
                    if (scrollAttempts == 2)
                    {
                        return products;
                    }
                }
                else
                {
                    scrollAttempts = 0;
                    numberOfItems = newNumberOfItems;
                }

                jsExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                System.Threading.Thread.Sleep(500);
                wait.Until(driver => (string)jsExecutor.ExecuteScript("return document.readyState") == "complete");
            }
            return null;
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
