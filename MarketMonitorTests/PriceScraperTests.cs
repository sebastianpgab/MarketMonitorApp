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

            var product = new Product()
            {
                Id = 1,
                Name = "Name",
                IdProduct = "1",
                Price = 23,
                IsNew = false
            };

            var productTwo = new Product()
            {
                Id = 1,
                Name = "Name",
                IdProduct = "1",
                Price = 23,
                IsNew = false
            };

            List<Product> productsFormFirstPage = new List<Product>();
            List<Product> productsFormSecondPage = new List<Product>();

            productsFormSecondPage.Add(productTwo);
            productsFormFirstPage.Add(product);

            string baseURl = "";
            int currentPage = 1;
            var distributor = new Distributor();
            _mockStrategySelector.Setup(p => p.ChoseStrategy(distributor)).Returns( new TwojaBronStrategy(_mockHtmlWebAdapter.Object));
            //problem with mocking 
            _mockStrategy.Setup(p => p.GetLastPageNumber(_mockHtmlWebAdapter.Object, baseURl)).Returns(2);
            _mockStrategy.SetupSequence(p => p.GetProducts(baseURl, currentPage))
                .Returns(productsFormFirstPage)
                .Returns(productsFormSecondPage);
                                       ;
            //Act
            var result = _priceScraper.GetProducts(baseURl, distributor);

        }
    }
}