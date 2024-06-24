using HtmlAgilityPack;

namespace MarketMonitorApp.Services
{
    public interface IHtmlWebAdapter
    {
        HtmlDocument Load(string url);
    }
    public class HtmlWebAdapter : IHtmlWebAdapter
    {
        private readonly HtmlWeb _htmlWeb;

        public HtmlWebAdapter(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        public HtmlDocument Load(string url)
        {
            return _htmlWeb.Load(url);
        }
    }
}
