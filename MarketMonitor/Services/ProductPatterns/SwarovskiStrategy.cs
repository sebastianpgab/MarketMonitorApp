﻿using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Collections.ObjectModel;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class SwarovskiStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public SwarovskiStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        public decimal CleanPrice(string price)
        {
            price = price.Substring(0, price.IndexOf(",")).Trim();
            return decimal.Parse(price);
        }

        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
        {
            return 1;
        }

        private void AcceptNewsletter(WebDriverWait wait, ChromeDriver driver, IJavaScriptExecutor jsExecutor)
        {
            try
            {
                var acceptNewsletterButton = wait.Until(driver => driver.FindElement(By.CssSelector("button.swo-css-1eqza22")));
                jsExecutor.ExecuteScript("arguments[0].click();", acceptNewsletterButton);
                Console.WriteLine("Newsletter window accepted");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("No newsletter banner present");
            }
        }

        public void ClickSideRightButton(ChromeDriver driver, WebDriverWait wait, IJavaScriptExecutor jsExecutor)
        {
            // Poczekaj aż przycisk będzie widoczny i klikalny
            var sideRightButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.side-right")));

            // Kliknij przycisk za pomocą JavaScript (jeśli zwykły Click() nie działa)
            jsExecutor.ExecuteScript("arguments[0].click();", sideRightButton);

            // Możesz również kliknąć w standardowy sposób:
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var products = new List<Product>();

            using (var driver = new ChromeDriver())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

                driver.Navigate().GoToUrl(baseUrl);
                AcceptNewsletter(wait, driver, jsExecutor);

                // Przejdź przez zagnieżdżone kategorie i serie
                products.AddRange(NavigateToSeriesOrProducts(driver, wait, jsExecutor));

                driver.Quit();
            }

            return products;
        }

        private IEnumerable<Product> NavigateToSeriesOrProducts(ChromeDriver driver, WebDriverWait wait, IJavaScriptExecutor jsExecutor)
        {
            var products = new List<Product>();

            Func<IWebDriver, ReadOnlyCollection<IWebElement>> findCategoryElements = drv =>
            {
                return drv.FindElements(By.CssSelector("li.with-image.family"));
            };

            for (int i = 0; i < findCategoryElements(driver).Count; i++)
            {
                var categoryElements = findCategoryElements(driver); 
                var categoryElement = categoryElements[i];

                try
                {
                    wait.Timeout = TimeSpan.FromSeconds(10);
                    var acceptNewsletterButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.swo-css-1eqza22")));

                    jsExecutor.ExecuteScript("arguments[0].click();", acceptNewsletterButton);

                    var sideRightButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.side-right")));

                    jsExecutor.ExecuteScript("arguments[0].click();", sideRightButton);

                    var clickableElement = wait.Until(drv => categoryElement.FindElement(By.CssSelector("div._picture")));

                    string previousUrl = driver.Url;

                    jsExecutor.ExecuteScript("arguments[0].click();", clickableElement);

                    wait.Until(driver => driver.Url != previousUrl);

                    Thread.Sleep(500);

                    products.AddRange(NavigateToProducts(driver, wait, jsExecutor));

                    driver.Navigate().GoToUrl(previousUrl);

                    wait.Until(drv => findCategoryElements(driver));
                }
                catch (StaleElementReferenceException)
                {
                    i--; 
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("No clickable element was found.");
                }
            }

            return products;
        }

        private IEnumerable<Product> NavigateToProducts(ChromeDriver driver, WebDriverWait wait, IJavaScriptExecutor jsExecutor)
        {
            var products = new List<Product>();

            var productElements = driver.FindElements(By.CssSelector(".with-image"));

            for (int i = 0; i < productElements.Count; i++)
            {
                productElements = driver.FindElements(By.CssSelector(".with-image"));
                string previousUrl = driver.Url;

                var clickableElement = productElements[i].FindElement(By.CssSelector("div._picture"));
                jsExecutor.ExecuteScript("arguments[0].click();", clickableElement);
                var sideRightButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.side-right")));

                sideRightButton.Click();


                var checkWariant = CheckIfElementVisible(wait);

                if (checkWariant)
                {

                    var prodactsFromWariatns = NavigateToWariants(driver, wait, jsExecutor, previousUrl);
                    products.AddRange(prodactsFromWariatns);
                }
                else
                {
                    var product = new Product
                    {
                        IdProduct = ExtractIdProduct(driver),
                        Name = ExtractProductName(driver),
                        Price = ExtractProductPrice(driver)
                    };
                    products.Add(product);

                    driver.Navigate().Back();
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".with-image")));
                }
            }
            return products;
        }

        private decimal ExtractProductPrice(ChromeDriver driver)
        {
            try
            {
                var priceElement = driver.FindElement(By.CssSelector("div.js-pdp-primary-action-above-the-fold-area"));
                string price = priceElement.Text;
                return CleanPrice(price);
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("No price found");
                return 0;
            }
        }

        private string ExtractProductName(ChromeDriver driver)
        {
            try
            {
                var nameElement = driver.FindElement(By.CssSelector("h1 span"));
                return nameElement.Text;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("No product name found");
                return string.Empty;
            }
        }

        private string ExtractIdProduct(ChromeDriver driver)
        {
            try
            {
                var idElement = driver.FindElement(By.CssSelector("li.active a.no-decoration")).GetAttribute("href");

                if (string.IsNullOrEmpty(idElement))
                {
                    Console.WriteLine("Href attribute is empty or null.");
                    return null;
                }

                var urlParts = idElement.Split('/');
                var productId = urlParts.Last();

                if (string.IsNullOrEmpty(productId))
                {
                    Console.WriteLine("Product ID is empty.");
                    return null;
                }

                return productId;
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Element not found: {ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"Null reference encountered: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return null;
        }

        private bool CheckIfElementVisible(WebDriverWait wait)
        {
            try
            {
                var element = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".gridless")));
                Console.WriteLine("Element 'gridless' found and is visible.");
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Element 'gridless' was not found or visible within the timeout.");
                return false;
            }
        }

        private IEnumerable<Product> NavigateToWariants(ChromeDriver driver, WebDriverWait wait, IJavaScriptExecutor javaScriptExecutor, string previousUrl)
        {
            List<Product> products = new List<Product>();
            var clickableElement = driver.FindElements(By.CssSelector(".js-mount li a"));
            var elements = driver.FindElements(By.CssSelector("li a"));

            for (int i = 0; i < clickableElement.Count; i++)
            {
                try
                {
                    var acceptNewsletterButton = driver.FindElement(By.CssSelector("button.swo-css-1eqza22"));
                    acceptNewsletterButton.Click();
                }
                catch (ElementNotInteractableException)
                {
                }
                catch (NoSuchWindowException)
                {
                }

                try
                {
                    javaScriptExecutor.ExecuteScript("arguments[0].click();", clickableElement[i]);

                    var sideRightButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.side-right")));
                    sideRightButton.Click();

                    for (int j = clickableElement.Count; j < elements.Count; j++)
                    {
                        var product = new Product
                        {
                            IdProduct = ExtractIdProduct(driver) + " " + clickableElement[i].Text + " " + elements[j].Text,
                            Name = ExtractProductName(driver) + " " + clickableElement[i].Text +" " +  elements[j].Text,
                            Price = ExtractProductPrice(driver)
                        };
                        products.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            driver.Navigate().GoToUrl(previousUrl);
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".with-image")));
            return products;
        }



    }
}