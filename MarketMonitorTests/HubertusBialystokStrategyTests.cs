using HtmlAgilityPack;
using MarketMonitorApp.Services.ProductPatterns;
using MarketMonitorApp.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MarketMonitorApp.Entities;

namespace MarketMonitorTests
{
    public class HubertusBialystokStrategyTests
    {
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly HubertusBialystokStrategy _hubertusBialystokStrategy;
        public HubertusBialystokStrategyTests()
        {
            _htmlDocument = new HtmlDocument();
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _hubertusBialystokStrategy = new HubertusBialystokStrategy(_mockHtmlWebAdapter.Object);
        }

        public static IEnumerable<object[]> GetLastPageNumberTestCases()
        {

            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='pagination'>
                            <li><a><<</a></li>
                            <li><a>1</a></li>
                            <li><a>2</a></li>
                            <li><a>3</a></li>
                            <li><a>>></a></li>
                        </div>
                    </body>
                </html>
                ",
                3
            };

            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='pagination'>
                            <li><a>1</a></li>
                            <li><a>>></a></li>
                        </div>
                    </body>
                </html>
                ",
                1
            };

            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='pagination'>
                        </div>
                    </body>
                </html>
                ",
                1
            };

            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='pagination'>
                            <li><a>Page 1</a></li>
                            <li><a>Page 2</a></li>
                            <li><a>Page 3</a></li>
                        </div>
                    </body>
                </html>
                ",
                1
            };
        }


        [Theory]
        [MemberData (nameof(GetLastPageNumberTestCases))]
        public void GetLastPageNumber_ShouldReturnCorrectLastPageNumber_WhenHtmlIsValid(string html, int pageNumber)
        {
            //Arrange
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _hubertusBialystokStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "");

            //Assert
            Assert.Equal(pageNumber, result);
        }

        [Fact]
        public void GetProducts_ShouldReturnCorrectProducts()
        {
            //Arrange
            var html = @"
            <html>
              <body>
                <div class ='abs-layout-product-gallery'>
                  <div class='abs-catalog-index'>123</div>
                  <div class='abs-product-name'>Pistolet CZ Shadow 2 OR 9 k. 9x19</div>
                  <div class='abs-item-price-amount'>6 850,50 zł</div>
                </div>
                <div class ='abs-layout-product-gallery'>
                  <div class='abs-catalog-index'>123</div>
                  <div class='abs-product-name'>Pistolet CZ </div>
                  <div class='abs-item-price-amount'>685 zł</div>
                </div>
                <div class ='abs-layout-product-gallery'>
                  <div class='abs-catalog-index'>5454-455P</div>
                  <div class='abs-product-name'>Pistolet CZ P-10 S k. 9x19</div>
                  <div class='abs-item-price-amount'>685.05zł</div>
                </div>
              /body>
            </html>";

            HtmlDocument _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Asct
            var result = _hubertusBialystokStrategy.GetProducts("url", 1);
            List<Product> products = new List<Product>(result);

            //Arrange
            Assert.Equal(2, products.Count);
            Assert.Equal("123", products[0].IdProduct);
            Assert.Equal("Pistolet CZ Shadow 2 OR 9 k. 9x19", products[0].Name);
            Assert.Equal(6850.50m, products[0].Price);
            Assert.Equal("5454-455P", products[1].IdProduct);
            Assert.Equal("Pistolet CZ P-10 S k. 9x19", products[1].Name);
            Assert.Equal(685.05m, products[1].Price);
        }

        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            //Arrange
            var html = @"
            <html>
              <body>
                <div class ='abs-layout-product-gallery'>
                  <div class='abs-catalog-index'>123</div>
                  <div class='abs-product-name'></div>
                  <div class='abs-item-price-amount'>6 850,50 zł</div>
                </div>
                <div class ='abs-layout-product-gallery'>
                  <div class='abs-catalog-index'>123</div>
                  <div class='abs-item-price-amount'>6 850,50 zł</div>
                </div>
              /body>
            </html>";

           
            HtmlDocument _htmlDocument = new HtmlDocument();
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => _hubertusBialystokStrategy.GetProducts(html, 1);

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetProducts_ShouldReturnEmptyList_WhenNoProductsInHtml()
        {
            // Arrange
            var html = "<html><body></body></html>";
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            // Act
            var result = _hubertusBialystokStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
            Assert.NotNull(result);
        }

    }
}
