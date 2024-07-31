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
    }
}
