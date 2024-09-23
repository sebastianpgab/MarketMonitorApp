using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

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

            // Znalezienie elementów kategorii, np. obrazek lub inny kontener, który prowadzi do szczegółów
            var categoryElements = driver.FindElements(By.CssSelector("li.with-image.family"));

            // Sprawdź, czy są zagnieżdżone kategorie lub serie
            if (categoryElements.Count > 0)
            {
                foreach (var categoryElement in categoryElements)
                {
                    try
                    {
                        var clickableElement = categoryElement.FindElement(By.CssSelector("div._picture"));
                        jsExecutor.ExecuteScript("arguments[0].click();", clickableElement);

                        // Poczekaj, aż załaduje się strona
                        System.Threading.Thread.Sleep(500);

                        // Sprawdź, czy są produkty na załadowanej stronie
                        products.AddRange(NavigateToProducts(driver, wait, jsExecutor));

                        // Powrót do poprzedniej strony
                        driver.Navigate().Back();
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Nie znaleziono elementu do kliknięcia.");
                    }
                }
            }
            else
            {
                // Jeżeli nie ma kategorii, przejdź bezpośrednio do produktów
                products.AddRange(NavigateToProducts(driver, wait, jsExecutor));
            }

            return products;
        }

        private IEnumerable<Product> NavigateToProducts(ChromeDriver driver, WebDriverWait wait, IJavaScriptExecutor jsExecutor)
        {
            var products = new List<Product>();

            // Zbierz wszystkie elementy, ale przetwarzaj je po indeksie, nie bezpośrednio na elementach DOM
            var productElements = driver.FindElements(By.CssSelector(".with-image"));
            int numberOfProducts = productElements.Count;

            for (int i = 0; i < numberOfProducts; i++)
            {
                // Znajdź element ponownie, by uniknąć błędu StaleElementReferenceException
                productElements = driver.FindElements(By.CssSelector(".with-image"));

                // Znajdź klikalny element i kliknij na produkt
                var clickableElement = productElements[i].FindElement(By.CssSelector("div._picture"));
                jsExecutor.ExecuteScript("arguments[0].click();", clickableElement);

                //Poczekaj
                var sideRightButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.side-right")));

                sideRightButton.Click();

                // Wydobądź dane produktu po nawigacji
                var product = new Product
                {
                    Name = ExtractProductName(driver),
                    Price = ExtractProductPrice(driver)
                };
                products.Add(product);

                // Wróć do listy produktów
                driver.Navigate().Back();

                // Oczekuj na ponowne załadowanie strony z produktami
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".with-image")));
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
    }
}
