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
    public class IncorsaStrategyTests
    {
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly IncorsaStrategy _incorsaStrategy;
        public IncorsaStrategyTests()
        {
            _htmlDocument = new HtmlDocument();
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _incorsaStrategy = new IncorsaStrategy(_mockHtmlWebAdapter.Object);
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
        [MemberData(nameof(GetLastPageNumberTestCases))]
        public void GetLastPageNumber_ShouldReturnCorrectLastPageNumber_WhenHtmlIsValid(string html, int pageNumber)
        {
            //Arrange
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _incorsaStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "url");

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
                    <div class='abs-product-name'>
                        <a href='/sztucer-bergara-b14-stroke-18-223-rem?c=169'>Sztucer Bergara B14 Stroke</div>
                    </div>
                   <div class='abs-item-price-final'>
                        <a>45 zł</a>
                    </div>
                    <div class='abs-product-name'>
                        <a href='/sztucer-bergara-b14-stroke-18-223-rem?c=169'>Sztucer Bergara B14 Stroke</div>
                    </div>
                   <div class='abs-item-price-final'>
                        <a>45 zł</a>
                    </div>
                    <div class='abs-product-name'>
                        <a href='/koszulka-blaser-xxl'>Koszulka Blaser XXL</div>
                    </div>
                   <div class='abs-item-price-final'>
                        <a>454.00 zł</a>
                    </div>
                </body>
            </html>";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _incorsaStrategy.GetProducts("url", 1);

            //Arrange
            List<Product> products = new List<Product>(result);

            Assert.Equal(2, products.Count);
            Assert.Equal("/sztucer-bergara-b14-stroke-18-223-rem?c=169", products[0].IdProduct);
            Assert.Equal("Sztucer Bergara B14 Stroke", products[0].Name);
            Assert.Equal(45.00m, products[0].Price);
            Assert.Equal("/koszulka-blaser-xxl", products[1].IdProduct);
            Assert.Equal("Koszulka Blaser XXL", products[1].Name);
            Assert.Equal(454.00m, products[1].Price);
        }

        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            string html =
            @"
            <html>
                <div class='abs-product-name'>
                    <a href='/sztucer-bergara-b14-stroke-18-223-rem?c=169'></div>
                </div>
                <div class='abs-item-price-final'>
                    <a>45 zł</a>
                </div>
            </html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => { _incorsaStrategy.GetProducts("url", 1); };

            //Assert
            Assert.Throws<NullReferenceException>(action);
        }

        [Fact]
        public void GetProducts_ShouldReturnEmptyList_WhenNoProductsInHtml()
        {
            // Arrange
            var html = "<html><body></body></html>";
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            // Act
            var result = _incorsaStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
            Assert.NotNull(result);
        }
    }
}
