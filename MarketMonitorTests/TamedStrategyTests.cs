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
    public class TamedStrategyTests
    {
        private readonly Mock<IHtmlWebAdapter> _htmlWebAdapterMock;
        private readonly HtmlDocument _htmlDocument;
        private readonly TamedStrategy _tamedStrategy;

        public TamedStrategyTests()
        {
            _htmlWebAdapterMock = new Mock<IHtmlWebAdapter>();
            _htmlDocument = new HtmlDocument();
            _tamedStrategy = new TamedStrategy(_htmlWebAdapterMock.Object);
        }

        public static IEnumerable<object[]> GetLastPageNumberTestCases()
        {
            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='page-list'>
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
                        <div class='page-list'>
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
                        <div class='page-list'>
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
                        <div class='page-list'>
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
        public void GetLastPageNumber_WhenHtmlIsValid_ShouldReturnCorrectLastPageNumber(string html, int expectedLastPageNumber)
        {
            // Arrange
            _htmlDocument.LoadHtml(html);
            _htmlWebAdapterMock.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            // Act
            var result = _tamedStrategy.GetLastPageNumber(_htmlWebAdapterMock.Object, "url");

            // Assert
            Assert.Equal(expectedLastPageNumber, result);
        }

        [Fact]
        public void GetProducts_ShouldReturnCorrectProducts()
        {
            //Arrange
            var html = @"
            <html>
                <body>
                    <div class='ajax_block_product'>
                        <div class='leo-more-info' data-idproduct='123'></div>
                        <div class='product-title'>
                            <a href='https://tamed.pl/glowna/infiray-tube-tl50'>InfiRay Eye Series V2.0 C2w</a>
                        </div>
                        <div class='price'>
                            <span itemprop='price'>200,00 zł</span>
                        </div>
                    </div>
                    <div class='ajax_block_product'>
                        <div class='leo-more-info' data-idproduct='32132-125'></div>
                        <div class='product-title'>
                            <a href='https://tamed.pl/termowizory-infiray/infiray-scl35w'>Infiray SCL35W</a>
                        </div>
                        <div class='price'>
                            <span itemprop='price'>5,50zł</span>
                        </div>
                    </div>
                </body>
            </html>";


            _htmlDocument.LoadHtml(html);
            _htmlWebAdapterMock.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _tamedStrategy.GetProducts("url", 1);

            //Assert
            List<Product> products = new List<Product>(result);
            Assert.NotNull(result);
            Assert.Equal(2, products.Count);
            Assert.Equal("123", products[0].IdProduct);
            Assert.Equal("infiray tube tl50", products[0].Name);
            Assert.Equal(200.00m, products[0].Price);
            Assert.Equal("32132-125", products[1].IdProduct);
            Assert.Equal("infiray scl35w", products[1].Name);
            Assert.Equal(5.50m, products[1].Price);
        }
    }
}
