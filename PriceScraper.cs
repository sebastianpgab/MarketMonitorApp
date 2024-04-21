using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MarketMonitorApp
{
    public interface IPriceScraper
    {
        public IEnumerable<Product> GetPrices(string baseUrl, Distributor distributor);

    }
    public class PriceScraper : IPriceScraper
    {
        private string BaseUrl;

        public IEnumerable<Product> GetPrices(string baseUrl, Distributor distributor)
        {
            BaseUrl = baseUrl;
            var allProducts = new List<Product>();
            decimal parsedPrice;
            var web = new HtmlWeb();
            var currentPage = 1;
            var lastPage = GetLastPageNumber(web);

            while (currentPage <= lastPage)
            {
                var pageUrl = $"{BaseUrl}?page={currentPage}";
                var document = web.Load(pageUrl);
                var products = document.DocumentNode.QuerySelectorAll(".product");

                foreach (var product in products)
                {
                    var productId = product.GetAttributeValue("data-product-id", string.Empty);
                    var productNameNode = product.QuerySelector(".prodname");
                    var priceElement = product.QuerySelector(".price em");
                    var productName = productNameNode.InnerText.Trim();
                    var price = priceElement.InnerText.Trim();

                    var newProduct = new Product();
                    newProduct.IdProduct = productId;
                    newProduct.Name = productName;
                    newProduct.DistributorId = distributor.Id;
                    newProduct.Distributor = distributor;
                    newProduct.Price = price;


                    allProducts.Add(newProduct);

                }
                currentPage++;
            }

            return allProducts;
        }

        private int GetLastPageNumber(HtmlWeb web)
        {
            var document = web.Load(BaseUrl);
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".paginator li a")
                                                        .Where(a => int.TryParse(a.InnerText.Trim(), out _))
                                                        .ToList();

            var lastPageNumber = 1; 

            if (paginationLinks.Any())
            {
                if (int.TryParse(paginationLinks.Last().InnerText.Trim(), out int pageNumber))
                {
                    lastPageNumber = pageNumber;
                }
            }

            return lastPageNumber;
        }
    }
}
