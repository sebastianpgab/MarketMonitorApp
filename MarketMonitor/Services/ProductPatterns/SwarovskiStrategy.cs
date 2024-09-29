using MarketMonitorApp.Entities;
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

            // Funkcja do dynamicznego znajdowania elementów kategorii
            Func<IWebDriver, ReadOnlyCollection<IWebElement>> findCategoryElements = drv =>
            {
                return drv.FindElements(By.CssSelector("li.with-image.family"));
            };

            // Użycie pętli for zamiast foreach
            for (int i = 0; i < findCategoryElements(driver).Count; i++)
            {
                var categoryElements = findCategoryElements(driver); // Ponowne pobieranie elementów kategorii
                var categoryElement = categoryElements[i];

                try
                {
                    wait.Timeout = TimeSpan.FromSeconds(10);
                    var acceptNewsletterButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.swo-css-1eqza22")));

                    jsExecutor.ExecuteScript("arguments[0].click();", acceptNewsletterButton);


                    // Znalezienie dynamicznie ładowanego elementu
                    var sideRightButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.side-right")));

                    // Kliknij przycisk za pomocą JavaScript
                    jsExecutor.ExecuteScript("arguments[0].click();", sideRightButton);

                    // Znajdź element do kliknięcia, upewniając się, że jest aktualny
                    var clickableElement = wait.Until(drv => categoryElement.FindElement(By.CssSelector("div._picture")));

                    // Zapisz URL
                    string previousUrl = driver.Url;

                    // Kliknięcie dynamicznie załadowanego elementu
                    jsExecutor.ExecuteScript("arguments[0].click();", clickableElement);

                    // Poczekaj na załadowanie nowej strony
                    wait.Until(driver => driver.Url != previousUrl);

                    // Znajdź produkty na nowej stronie
                    products.AddRange(NavigateToProducts(driver, wait, jsExecutor));

                    // Powrót do poprzedniej strony
                    driver.Navigate().GoToUrl(previousUrl);

                    // Ponowne wyszukanie elementów kategorii po powrocie
                    wait.Until(drv => findCategoryElements(driver));
                }
                catch (StaleElementReferenceException)
                {
                    // Jeśli element jest nieaktualny, ponowne wyszukiwanie elementów dynamicznie
                    Console.WriteLine("Element stał się nieaktualny, ponowne wyszukiwanie.");
                    i--; // Powrót do poprzedniego indeksu, aby spróbować ponownie
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("Nie znaleziono elementu do kliknięcia.");
                }
            }

            return products;
        }

        private IEnumerable<Product> NavigateToProducts(ChromeDriver driver, WebDriverWait wait, IJavaScriptExecutor jsExecutor)
        {
            var products = new List<Product>();

            // Zbierz wszystkie elementy, ale przetwarzaj je po indeksie, nie bezpośrednio na elementach DOM
            var productElements = driver.FindElements(By.CssSelector(".with-image"));

            for (int i = 0; i < productElements.Count; i++)
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
                    IdProduct = ExtractIdProduct(driver),
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

        private string ExtractIdProduct(ChromeDriver driver)
        {
            try
            {
                // Znajdź element zawierający identyfikator produktu
                var idElement = driver.FindElement(By.CssSelector("li.active a.no-decoration")).GetAttribute("href");

                // Sprawdź, czy znaleziony atrybut href nie jest pusty
                if (string.IsNullOrEmpty(idElement))
                {
                    Console.WriteLine("Href attribute is empty or null.");
                    return null;
                }

                // Rozdziel URL na części i zwróć ostatni element (id produktu)
                var urlParts = idElement.Split('/');
                var productId = urlParts.Last();

                // Zweryfikuj, czy id produktu nie jest puste
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

            // W przypadku jakiegokolwiek błędu zwracamy null
            return null;
        }

    }
}