using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Globalization;
using System.Text.RegularExpressions;
using System;


namespace MarketMonitorApp.Services.ProductPatterns
{
    public class DeltaOpticalStrategy : IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage, HtmlDocument document)
        {
            // Konfiguracja ścieżki do ChromeDriver
            var service = ChromeDriverService.CreateDefaultService();
            var options = new ChromeOptions();
            options.AddArguments("headless"); // Uruchomienie przeglądarki w trybie bezgłowym

            // Inicjalizacja WebDriver
            using (var driver = new ChromeDriver(service, options))
            {
                try
                {
                    // Przechodzenie do strony
                    driver.Navigate().GoToUrl("https://deltaoptical.pl/termowizory");
                    Thread.Sleep(2000); // Odczekanie, aby strona mogła załadować dane na początku

                    // Symulacja przewijania strony
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    int numberOfItems = 0;
                    while (true)
                    {
                        var newNumberOfItems = driver.FindElements(By.CssSelector("li[data-product-id]")).Count;
                        if (newNumberOfItems == numberOfItems)
                            break; // Przerwanie, gdy liczba elementów się nie zmienia
                        numberOfItems = newNumberOfItems;
                        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                        Thread.Sleep(2000); // Odczekanie na załadowanie nowych danych
                    }

                    // Zbieranie danych produktów
                    var products = driver.FindElements(By.CssSelector("li[data-product-id]"));
                    var productList = new List<(string Id, string Name, string Price)>();

                    foreach (var productElement in products)
                    {
                        string productId = productElement.GetAttribute("data-product-id");
                        string productName = productElement.FindElement(By.CssSelector("h2.product-name > a")).GetAttribute("title");
                        string productPrice = productElement.FindElement(By.CssSelector(".product-column .price")).Text;
                        productList.Add((productId, productName, productPrice));
                    }

                    // Wyświetlanie informacji o wszystkich produktach
                    //spróbuj tego: document.querySelector("ul.product-list > li div.price-container > span.price")

                    foreach (var product in productList)
                    {
                        Console.WriteLine($"ID produktu: {product.Id}, Nazwa produktu: {product.Name}, Cena produktu: {product.Price}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Wystąpił błąd: " + ex.Message);
                }
                finally
                {
                    driver.Quit(); // Zamknięcie przeglądarki
                }
            }
            return null;
        }
    }
}
