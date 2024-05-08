using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class DeltaOpticalStrategy : IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage, HtmlDocument document)
        {
            // Konfiguracja ścieżki do ChromeDriver
            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            var options = new ChromeOptions();
            options.AddArguments("headless"); // Uruchomienie przeglądarki w trybie bezgłowym

            // Inicjalizacja WebDriver
            using (var driver = new ChromeDriver(service, options))
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    // Przejdź na stronę z produktami
                    driver.Navigate().GoToUrl("https://deltaoptical.pl/termowizory");
                    IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                    wait.Until(driver => (string)jsExecutor.ExecuteScript("return document.readyState") == "complete");

                    // Akceptacja plików cookie za pomocą JavaScript
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

                    // Symulacja przewijania strony
                    int numberOfItems = 0;
                    while (true)
                    {
                        // problem jest taki ze zbiera tylko 9 produktów
                        var newNumberOfItems = driver.FindElements(By.CssSelector("li[data-product-id]")).Count;
                        if (newNumberOfItems == numberOfItems)
                            break; // Przerwanie, gdy liczba elementów się nie zmienia
                        numberOfItems = newNumberOfItems;
                        jsExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                        wait.Until(driver => (string)jsExecutor.ExecuteScript("return document.readyState") == "complete");
                    }

                    var products = driver.FindElements(By.CssSelector("li[data-product-id]"));
                    var productList = new List<(string Id, string Name, string Price)>();

                    foreach (var productElement in products)
                    {
                        string productId = productElement.GetAttribute("data-product-id");
                        string productName = productElement.FindElement(By.CssSelector("h2.product-name > a")).GetAttribute("title");
                        string productPrice = productElement.FindElement(By.CssSelector(".product-column .price")).Text;
                        productList.Add((productId, productName, productPrice));
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

            return null;
        }
    }
}