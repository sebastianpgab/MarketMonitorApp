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
    public class KniejaStrategyTests 
    {
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly KniejaStrategy _kniejaStrategy;
        public KniejaStrategyTests()
        {
            _htmlDocument = new HtmlDocument();
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _kniejaStrategy = new KniejaStrategy(_mockHtmlWebAdapter.Object);
        }

        public static IEnumerable<object[]> GetLastPageNumberTestCases()
        {
            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <ul>
                            <li class='page-item active'><a class='page-link'><<</a></li>
                            <li class='page-item'><a class='page-link'>1</a></li>
                            <li class='page-item'><a class='page-link'>2</a></li>
                            <li class='page-item'><a class='page-link'>3</a></li>
                            <li class='page-item'><a class='page-link'>>></a></li>
                        </ul>
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
                        <ul>
                            <li class='page-item active'><a class='page-link'><<</a></li>
                            <li class='page-item'><a class='page-link'>1</a></li>
                            <li class='page-item'><a class='page-link'>>></a></li>
                        </ul>
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
                        </ul>
                            <li class='page-item'><a class='page-link'></a></li>
                        </ul>   
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
                        <ul>
                            <li class='page-item active'><a class='page-link'><<</a></li>
                            <li class='page-item'><a class='page-link'>Page 1</a></li>
                            <li class='page-item'><a class='page-link'>Page 2</a></li>
                            <li class='page-item'><a class='page-link'>>></a></li>
                        </ul>
                    </body>
                </html>
                ",
                1
            };
        }

        [Theory]
        [MemberData (nameof(GetLastPageNumberTestCases))]
        public void GetLastPageNumber_ShouldReturnCorrect_WhenHtmlIsValid(string html, int pageNumber)
        {
            //Arrange
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _kniejaStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "url");

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
                    <div class='product-miniature' data-id-product='123'>
                        <div class='product-title'>RP Arms Karabin kulowy</div>
                        <div class='price'>45 zł</div>
                    </div>
                    <div class='product-miniature' data-id-product='565'>
                        <div class='product-title'>RP Arms Sztucer kal. 30-06</div>
                        <div class='price'>76 zł</div>
                    </div>
                    <div class='product-miniature' data-id-product='565'>
                        <div class='product-title'>RP Arms Sztucer kal. 30-06</div>
                        <div class='price'>76 zł</div>
                    </div>
                </body>
            </html>";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _kniejaStrategy.GetProducts("url", 1);

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
            string html =
            @"
            <html>
                <div class='product-miniature' data-id-product='565'>
                    <div class='price'>76 zł</div>
                </div>
            </html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => { _kniejaStrategy.GetProducts("url", 1); };

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
            var result = _kniejaStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
            Assert.NotNull(result);
        }

    }
}
