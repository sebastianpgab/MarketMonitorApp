using HtmlAgilityPack;
using MarketMonitorApp.Services;
using MarketMonitorApp.Services.ProductPatterns;
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

        public TaniePolowanieStrategyTests()
        {
            _mockWebAdapter = new Mock<IHtmlWebAdapter>();
            _taniePolowanieStrategy = new TaniePolowanieStrategy(_mockWebAdapter.Object);
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

        [Theory]
        [MemberData(nameof(GetProducts))]
        public void GetProducts_ShouldReturnExpectedProducts(string html, int expectedResults)
        {
            // Arrange
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            _mockWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(htmlDoc);

            // Act
            var result = _taniePolowanieStrategy.GetProducts("url", 1);

            // Assert
            Assert.Equal(expectedResults, result.Count());
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
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            _mockWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(htmlDoc);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _taniePolowanieStrategy.GetProducts("url", 1));
        }
    }
}
