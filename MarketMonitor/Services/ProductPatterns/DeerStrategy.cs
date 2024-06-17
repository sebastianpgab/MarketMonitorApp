using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class DeerStrategy : IDistributorStrategy
    {
        //Logika do poprawy
        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.QuerySelectorAll(".page-numbers li a").ToList();

            if (paginationLinks.Count == 0)
            {
                return 1;
            }

            var lastPageString = paginationLinks[paginationLinks.Count - 2].InnerText.Trim();

            if (int.TryParse(lastPageString, out int lastPageNumber))
            {
                return lastPageNumber;
            }
            else
            {
                return 1;
            }
        }

        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var web = new HtmlWeb();
            var pageUrl = UpdatePageNumberInLink(baseUrl, currentPage);
            var document = web.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".product-grid-item");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.GetAttributeValue("data-id", string.Empty);
                var productName = productNode.QuerySelector(".wd-entities-title").InnerText.Trim();
                var priceElement = productNode.QuerySelector(".price")?.InnerText.Trim();

                if (string.IsNullOrWhiteSpace(priceElement))
                {
                    continue; 
                }

                var prices = DividePrice(priceElement);
                if (prices.Count > 1)
                {
                    AddProductsForMultiplePrices(products, productId, productName, prices);
                }
                else if (prices.Count == 1)
                {
                    AddProduct(products, productId, productName, prices.First());
                }
            }

            return products;
        }

        private void AddProductsForMultiplePrices(List<Product> products, string productId, string productName, List<decimal> prices)
        {
            int indexAdder = 0;
            foreach (var price in prices)
            {
                var newProduct = new Product
                {
                    IdProduct = productId + indexAdder,
                    Name = productName,
                    Price = price
                };
                products.Add(newProduct);
                indexAdder++;
            }
        }

        private void AddProduct(List<Product> products, string productId, string productName, decimal price)
        {
            var newProduct = new Product
            {
                IdProduct = productId,
                Name = productName,
                Price = price
            };
            products.Add(newProduct);
        }

        public List<decimal> DividePrice(string price)
        {
            var prices = price.Split('–');
            var cleanedPrices = new List<decimal>();

            foreach (var pr in prices)
            {
                var cleanPrice = CleanPrice(pr);
                if (decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal newPrice))
                {
                    cleanedPrices.Add(newPrice);
                }
            }
            return cleanedPrices;
        }

        private string CleanPrice(string price)
        {
            return Regex.Replace(price, @"\s+|zł|&nbsp;|<[^>]+>", "").Replace(",", ".");
        }

        private string UpdatePageNumberInLink(string baseUrl, int currentPage)
        {
            var segments = baseUrl.Split('/');
            if (segments.Length > 4)
            {
                segments[4] = currentPage.ToString();
            }
            return string.Join("/", segments);
        }

    }
}
