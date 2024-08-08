using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services;
using MarketMonitorApp.Services.ProductPatterns;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MarketMonitorTests
{
    public class RParmsStrategyTests
    {
        private readonly RParmsStrategy _rparmsStrategy;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly HtmlDocument _htmlDocument;
        public RParmsStrategyTests()
        {
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _rparmsStrategy = new RParmsStrategy(_mockHtmlWebAdapter.Object);
            _htmlDocument = new HtmlDocument();
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
            var result = _rparmsStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "url");

            //Assert
            Assert.Equal(pageNumber, result );
        }

        [Fact]
        public  void GetProducts_ShouldReturnCorrectProducts()
        {
            //Arrange
            var html = @"
            <html>
                <body>
                    <div class='item-product'>
                        <div class='product-miniature' data-id-product='123'></div>
                        <div class='product_name'>RP Arms Karabin kulowy</div>
                        <div class='price'>45 zł</div>
                    </div>
                    <div class='item-product'>
                        <div class='product-miniature' data-id-product='565'></div>
                        <div class='product_name'>RP Arms Sztucer kal. 30-06</div>
                        <div class='price'>76 zł</div>
                    </div>
                    <div class='item-product'>
                        <div class='product-miniature' data-id-product='565'></div>
                        <div class='product_name'>RP Arms Sztucer kal. 30-06</div>
                        <div class='price'>76 zł</div>
                    </div>
                </body>
            </html>";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _rparmsStrategy.GetProducts("url", 1);

            //Arrange
            List<Product> products = new List<Product>(result);

            Assert.Equal(2, products.Count);
            Assert.Equal("123", products[0].IdProduct);
            Assert.Equal("RP Arms Karabin kulowy", products[0].Name);
            Assert.Equal(45.00m, products[0].Price);
            Assert.Equal("565", products[1].IdProduct);
            Assert.Equal("RP Arms Sztucer kal. 30-06", products[1].Name);
            Assert.Equal(76.00m, products[1].Price);
        }

        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            // Arrange
            var html = @"
            <html>
                <body>
                    <div class='item-product'>
                        <div class='product-miniature' data-id-product='123'></div>
                        <div class='price'>45 zł</div>
                    </div>
                </body>
            </html>";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            // Act
            Action action = () => { _rparmsStrategy.GetProducts("url", 1); };

            // Assert
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
            var result = _rparmsStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
        }

    }
}
