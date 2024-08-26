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
    public  class KaliberStrategyTests
    {
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly KaliberStrategy _kaliberStrategy;
        public KaliberStrategyTests()
        {
            _htmlDocument = new HtmlDocument();
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _kaliberStrategy = new KaliberStrategy(_mockHtmlWebAdapter.Object);
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
            var result = _kaliberStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "url");

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
                    <div class='ajax_block_product'>
                        <div class='add_to_compare' data-id-product='123'></div>
                        <div class='product-name'>RP Arms Karabin kulowy</div>
                        <div class='price'>45 zł</div>
                    </div>
                    <div class='ajax_block_product'>
                        <div class='add_to_compare' data-id-product='123'></div>
                        <div class='product-name'>RP Arms Karabin kulowy</div>
                        <div class='price'>45 zł</div>
                    </div>
                    <div class='ajax_block_product'>
                        <div class='add_to_compare' data-id-product='1431'></div>
                        <div class='product-name'>Nóż taktyczny Mora</div>
                        <div class='price'>1299.00zł</div>
                    </div>
                </body>
            </html>";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _kaliberStrategy.GetProducts("url", 1);

            //Arrange
            List<Product> products = new List<Product>(result);

            Assert.Equal(2, products.Count);
            Assert.Equal("123", products[0].IdProduct);
            Assert.Equal("RP Arms Karabin kulowy", products[0].Name);
            Assert.Equal(45.00m, products[0].Price);
            Assert.Equal("1431", products[1].IdProduct);
            Assert.Equal("Nóż taktyczny Mora", products[1].Name);
            Assert.Equal(1299.00m, products[1].Price);
        }

        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            string html =
            @"
            <html>
              <div class='ajax_block_product'>
                <div class='add_to_compare' data-id-product='123'></div>
                <div class='price'>1299.00zł</div>
              </div>
            </html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => { _kaliberStrategy.GetProducts("url", 1); };

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
            var result = _kaliberStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
            Assert.NotNull(result);
        }
    }
}
