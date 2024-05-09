using HtmlAgilityPack;
using MarketMonitorApp.Entities;

namespace MarketMonitorApp.Services.ProductsStrategy
{
    public interface IDistributorStrategy
    {
        public IEnumerable<Product> GetProducts(string baseUrl, int currentPage);
        public int GetLastPageNumber(HtmlWeb web, string baseUrl);


    }
}
