using MarketMonitorApp;
using MarketMonitorApp.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using Xunit;
using Moq;
using static System.Net.WebRequestMethods;
using MarketMonitorApp.Services.ProductsStrategy;
using HtmlAgilityPack;
using MarketMonitorApp.Services.ProductPatterns;
using MarketMonitorApp.Services;

namespace MarketMonitorTests
{
    public class PriceScraperTests
    {
        private readonly Mock<IDistributorStrategySelector> _mockStrategySelector;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly Mock<IDistributorStrategy> _mockStrategy;
        private readonly PriceScraper _priceScraper;
        public PriceScraperTests()
        {
            _mockStrategySelector = new Mock<IDistributorStrategySelector>();
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _mockStrategy = new Mock<IDistributorStrategy>();
            _priceScraper = new PriceScraper(_mockStrategySelector.Object, _mockHtmlWebAdapter.Object);
        }

        [Fact]
        public void GetProducts_ReturnsAllProducts_WhenCalled()
        {
            // Arrange
            string baseUrl = "https://kaliber.pl/39_tikka?p=";
            var distributor = new Distributor();
            var productsPage1 = new List<Product>
            {
                new Product { IdProduct = "1", Name = "Product1", Price = 10 },
                new Product { IdProduct = "2", Name = "Product2", Price = 20 }
            };
            var productsPage2 = new List<Product>
            {
                new Product { IdProduct = "3", Name = "Product3", Price = 30 },
                new Product { IdProduct = "4", Name = "Product4", Price = 40 }
            };

            _mockStrategySelector.Setup(s => s.ChoseStrategy(distributor)).Returns(_mockStrategy.Object);
            _mockStrategy.Setup(s => s.GetLastPageNumber(It.IsAny<IHtmlWebAdapter>(), baseUrl)).Returns(2);
            _mockStrategy.SetupSequence(s => s.GetProducts(baseUrl, 1)).Returns(productsPage1);
            _mockStrategy.SetupSequence(s => s.GetProducts(baseUrl, 2)).Returns(productsPage2);

            // Act
            var result = _priceScraper.GetProducts(baseUrl, distributor);

            // Assert
            Assert.Equal(4, result.Count());
            Assert.Contains(result, p => p.IdProduct == "1");
            Assert.Contains(result, p => p.IdProduct == "2");
            Assert.Contains(result, p => p.IdProduct == "3");
            Assert.Contains(result, p => p.IdProduct == "4");
        }
    }
}