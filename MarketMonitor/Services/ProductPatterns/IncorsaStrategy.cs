using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public class IncorsaStrategy : IDistributorStrategy
    {
        private readonly IHtmlWebAdapter _htmlWeb;

        public IncorsaStrategy(IHtmlWebAdapter htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl)
        {
            var document = web.Load(baseUrl);
            var paginationLinks = document.DocumentNode.QuerySelectorAll(".pagination li a")
                                                        .Where(a => int.TryParse(a.InnerText.Trim(), out _))
                                                        .ToList();
            if (paginationLinks.Count == 0)
            {
                return 1;
            }

            var lastPageString = paginationLinks[paginationLinks.Count - 1].InnerText.Trim();

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
            var document = FetchHtmlDocument(baseUrl, currentPage);
            return ParseProducts(document);
        }

        private HtmlDocument FetchHtmlDocument(string baseUrl, int currentPage)
        {
            var pageUrl = baseUrl;
            if (currentPage != 1)
            {
                pageUrl = ChangePageNumber(baseUrl, currentPage);
            }
            return _htmlWeb.Load(pageUrl);
        }

        private IEnumerable<Product> ParseProducts(HtmlDocument document)
        {
            var products = new List<Product>();
            var imgDetailsNodes = document.DocumentNode.QuerySelectorAll(".abs-layout-img-and-details");
            var purchaseDetailsNodes = document.DocumentNode.QuerySelectorAll(".abs-layout-purchase");

            var pairedNodes = imgDetailsNodes.Zip(purchaseDetailsNodes, (imgNode, purchaseNode) => new
            {
                ImgDetails = imgNode,
                PurchaseDetails = purchaseNode
            }).ToList();

            foreach (var pair in pairedNodes)
            {
                var productId = pair.ImgDetails.QuerySelector(".abs-p-catalog-index > span:nth-child(2)").InnerText.Trim();
                var productName = pair.ImgDetails.QuerySelector(".abs-product-name a").InnerText.Trim();
                var priceElement = pair.PurchaseDetails.QuerySelector(".abs-item-price-amount");

                var price = priceElement == null ? "0" : priceElement.InnerText.Trim();

                var newProduct = new Product
                {
                    IdProduct = productId,
                    Name = productName,
                    Price = CleanPrice(price)
                };

                products.Add(newProduct);
            }

            return products;
        }
        private string ChangePageNumber(string url, int newPageNumber)
        {
            int pageIndex = url.IndexOf("page=");
            if (pageIndex == -1)
            {
                throw new ArgumentException("URL does not contain 'page=' parameter");
            }

            int pageEndIndex = url.IndexOf('&', pageIndex);
            if (pageEndIndex == -1)
            {
                pageEndIndex = url.Length;
            }

            string newUrl = url.Substring(0, pageIndex + 5) + newPageNumber + url.Substring(pageEndIndex);

            return newUrl;
        }

        public decimal CleanPrice(string price)
        {
            if (string.IsNullOrWhiteSpace(price))
            {
                return 0;
            }

            string cleanPrice = Regex.Replace(price, @"\s+|zł", "").Replace(",", ".").Replace("brutto", "");

            if (decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal newPrice))
            {
                return newPrice;
            }
            else
            {
                throw new FormatException($"The price '{price}' is not in a valid format.");
            }
        }
    }
}
