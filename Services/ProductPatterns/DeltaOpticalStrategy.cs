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

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class DeltaOpticalStrategy : IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            options.AddArguments("headless");
            var productsList = new List<Product>();

            using (var driver = new ChromeDriver(service, options))
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    driver.Navigate().GoToUrl(baseUrl);
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                    wait.Until(driver => (string)jsExecutor.ExecuteScript("return document.readyState") == "complete");

                    // Akceptacja plików cookie za pomocą JavaScript
                    AkcceptCookieFiles(wait, driver, jsExecutor);

                    // Symulacja przewijania strony
                    var products = SimulateScroll(wait, driver, jsExecutor);

                    foreach (var productElement in products)
                    {
                        string productId = productElement.GetAttribute("data-product-id");
                        string productName = productElement.FindElement(By.CssSelector("h2.product-name > a")).GetAttribute("title").Trim();
                        string productPrice = productElement.FindElement(By.CssSelector(".product-column .price")).Text.Trim();

                        decimal newPrice;
                        string cleanPrice = Regex.Replace(productPrice, @"\s+|zł", "").Replace(",", ".");
                        bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                        var product = new Product
                        {
                            IdProduct = productId,
                            Name = productName,
                            Price = newPrice
                        };

                        productsList.Add(product);
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

        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            return 1;
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
                products = driver.FindElements(By.CssSelector("li[data-product-id]"));
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
                System.Threading.Thread.Sleep(2000);
                wait.Until(driver => (string)jsExecutor.ExecuteScript("return document.readyState") == "complete");
            }

            return null;
        }

    }
}
