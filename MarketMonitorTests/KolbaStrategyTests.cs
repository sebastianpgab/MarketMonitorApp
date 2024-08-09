using HtmlAgilityPack;
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
    public class KolbaStrategyTests
    {
        private readonly KolbaStrategy _kolbaStrategy;
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _htmlWebAdapter;
        public KolbaStrategyTests()
        {
            _htmlDocument = new HtmlDocument();
            _htmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _kolbaStrategy = new KolbaStrategy(_htmlWebAdapter.Object);
        }

        [Theory]
        [InlineData("Załaduj następne (1 z 79)", 4)]
        [InlineData("Załaduj następne (1 z 72)", 3)]
        [InlineData("Załaduj następne (1 z 35)", 2)]
        [InlineData("Załaduj następne (1 z 100)", 4)]
        [InlineData("Załaduj następne (1 z 1)", 2)]
        [InlineData("Załaduj następne (1 z 37)", 3)]
        [InlineData("Załaduj następne (1 z 0)", 1)]
        [InlineData("Załaduj następne (1 z 108)", 4)]
        [InlineData("", 1)]

        public void GetLastPageNumber_ShouldReturnCorrect_WhenHtmlIsValid(string buttonText, int numberOfpages)
        {
            // Arrange
            _htmlWebAdapter.Setup(web => web.Load(It.IsAny<string>()))
                       .Returns(() =>
                       {
                           _htmlDocument.LoadHtml($"<button>{buttonText}</button>");
                           return _htmlDocument;
                       });
            // Act
            int lastPageNumber = _kolbaStrategy.GetLastPageNumber(_htmlWebAdapter.Object, "url");

            // Assert
            Assert.Equal(numberOfpages, lastPageNumber);
        }
    }
}
