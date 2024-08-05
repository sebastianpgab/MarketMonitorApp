using HtmlAgilityPack;
using MarketMonitorApp.Services;
using MarketMonitorApp.Services.ProductPatterns;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MarketMonitorTests
{
    public class TaniePolowanieStrategyTests
    {
        private readonly Mock<IHtmlWebAdapter> _mockWebAdapter;
        private readonly TaniePolowanieStrategy _taniePolowanieStrategy;
        private readonly HtmlDocument _htmlDoc;

        public TaniePolowanieStrategyTests()
        {
            _mockWebAdapter = new Mock<IHtmlWebAdapter>();
            _taniePolowanieStrategy = new TaniePolowanieStrategy(_mockWebAdapter.Object);
            _htmlDoc = new HtmlDocument();
        }

        public static IEnumerable<object[]> GetProducts()
        {
            yield return new object[]
            {
                @"
                <html>
                    <div class='product' data-product-id='1'>
                        <div class='productname'>Product 1</div>
                        <div class='price'><em>500,00 zł</em></div>
                    </div>
                    <div class='product' data-product-id='2'>
                        <div class='productname'>Product 2</div>
                        <div class='price'><em></em></div>
                    </div>
                </html>",
                2
            };
            yield return new object[]
            {
                "",
                0
            };
        }

        public static IEnumerable<object[]> GetLastPageNumberTestData()
        {
            yield return new object[]
            {
            @"
            <html>
                <body>
                    <div class='paginator'>
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
                    <div class='paginator'>
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
                    <div class='paginator'>
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
                    <div class='paginator'>
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
        [MemberData(nameof(GetProducts))]
        public void GetProducts_ShouldReturnExpectedProducts(string html, int expectedResult)
        {
            // Arrange
            _htmlDoc.LoadHtml(html);
            _mockWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDoc);

            // Act
            var result = _taniePolowanieStrategy.GetProducts("url", 1);

            // Assert
            Assert.Equal(expectedResult, result.Count());
        }

        [Fact]
        public void GetProducts_ThrowNullReferenceException()
        {
            var html = @"
            <html>
                <div class='product' data-product-id='1'>
                </div>
            </html>";

            // Arrange
            _htmlDoc.LoadHtml(html);
            _mockWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDoc);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _taniePolowanieStrategy.GetProducts("url", 1));
        }

        [Theory]
        [MemberData(nameof(GetLastPageNumberTestData))]
        public void GetLastPageNumber(string html, int expectedResult)
        {
            //Arrange
            _htmlDoc.LoadHtml(html);
            _mockWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDoc);

            //Act
            var result = _taniePolowanieStrategy.GetLastPageNumber(_mockWebAdapter.Object, "url");

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetProducts_ShouldReturnEmptyList_WhenNoProductsInHtml()
        {
            var html =
            @"
             <html>
                 <div class='no-products'></div>
             </html>
            ";

            _htmlDoc.LoadHtml(html);
            _mockWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDoc);

            var result = _taniePolowanieStrategy.GetProducts("url", 1);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
