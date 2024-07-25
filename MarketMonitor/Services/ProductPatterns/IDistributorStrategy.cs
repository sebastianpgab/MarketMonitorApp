using HtmlAgilityPack;
using MarketMonitorApp.Entities;

namespace MarketMonitorApp.Services.ProductsStrategy
{
    public interface IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage);
        public int GetLastPageNumber(IHtmlWebAdapter web, string baseUrl);
        public decimal CleanPrice(string price);
    }
}
