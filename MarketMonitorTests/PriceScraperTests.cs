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

        public static IEnumerable<object[]> GetProductsTestData()
        {
            yield return new object[]
            {
                new Product()
                {
                    Id = 1,
                    Name = "Name",
                    IdProduct = "1",
                    Price = 23,
                    IsNew = false
                },
                new Product()
                {
                    Id = 2,
                    Name = "Name",
                    IdProduct = "2",
                    Price = 25,
                    IsNew = true
                },
                new Product()
                {
                    Id = 2,
                    Name = "Name",
                    IdProduct = "2",
                    Price = 25,
                    IsNew = true
                }
            };
            
        }

        [Theory]
        [MemberData(nameof(GetProductsTestData))]
        public void GetProducts_ShouldReturnAllProducts_WhenCalled(Product firstProduct, Product secondProduct, Product thirdProduct)
        {
            // Arrange
            var firstPageProducts = new List<Product> { firstProduct };
            var secondPageProducts = new List<Product> { secondProduct };
            var thirdPageProducts = new List<Product> { thirdProduct };

            string baseUrl = "";
            var distributor = new Distributor();

            _mockStrategySelector.Setup(selector => selector.ChoseStrategy(distributor)).Returns(_mockStrategy.Object);

            _mockStrategy.Setup(strategy => strategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, baseUrl)).Returns(3);
            _mockStrategy.SetupSequence(strategy => strategy.GetProducts(baseUrl, It.IsAny<int>()))
                .Returns(firstPageProducts)
                .Returns(secondPageProducts)
                .Returns(thirdPageProducts);

            // Act
            var result = _priceScraper.GetProducts(baseUrl, distributor);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, product => product == firstProduct);
            Assert.Contains(result, product => product == secondProduct);
            Assert.Contains(result, product =>
                product.Id == thirdProduct.Id &&
                product.Name == thirdProduct.Name &&
                product.Price == thirdProduct.Price &&
                product.IsNew == thirdProduct.IsNew);
        }

    }
}