using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

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
            price = price.Replace("zł", "").Trim();
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
                    // Kliknięcie elementu zamiast szukania href
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
            var productLinks = driver.FindElements(By.CssSelector(".product-tile a"));

            foreach (var productLink in productLinks)
            {
                string productUrl = productLink.GetAttribute("href");
                driver.Navigate().GoToUrl(productUrl);

                var product = new Product
                {
                    Name = ExtractProductName(driver),
                    Price = ExtractProductPrice(driver)
                };
                products.Add(product);

                // Wróć do listy produktów
                driver.Navigate().Back();
            }

            return products;
        }

        private decimal ExtractProductPrice(ChromeDriver driver)
        {
            try
            {
                var priceElement = driver.FindElement(By.CssSelector(".price"));
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
                var nameElement = driver.FindElement(By.CssSelector(".product-name"));
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
