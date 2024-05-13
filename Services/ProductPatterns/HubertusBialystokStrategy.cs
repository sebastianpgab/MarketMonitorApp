using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class HubertusBialystokStrategy : IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage)
        {
            var web = new HtmlWeb();
            var pageUrl = $"{baseUrl}";
            var document = web.Load(pageUrl);

            var products = new List<Product>();
            var productNodes = document.DocumentNode.QuerySelectorAll(".abs-layout-product-gallery");

            foreach (var productNode in productNodes)
            {
                var productId = productNode.QuerySelector("abs-catalog-index").ToString();
                var productNameNode = productNode.QuerySelector(".abs-product-name");
                var priceElement = productNode.QuerySelector(".abs-item-price-amount");

                var productName = productNameNode.InnerText.Trim();
                var cleanPrice = priceElement != null ? System.Net.WebUtility.HtmlDecode(priceElement.InnerText).Replace(" zł", "").Replace(",", ".").Trim() : "Price not found";

                decimal newPrice;
                bool result = decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out newPrice);

                var newProduct = new Product();
                newProduct.IdProduct = productId;
                newProduct.Name = productName;
                newProduct.Price = newPrice;

                products.Add(newProduct);

            }

            return products;
        }

        public int GetLastPageNumber(HtmlWeb web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".pagination li a")
                                                        .Where(a => int.TryParse(a.InnerText.Trim(), out _))
                                                        .ToList();
            var lastPageNumber = 1;

            if (paginationLinks.Count > 1)
            {
                string lastPageNumberString = paginationLinks[paginationLinks.Count - 1].InnerHtml; 

                lastPageNumber = int.Parse(lastPageNumberString);
            }



            return lastPageNumber;
        }
    }
}
