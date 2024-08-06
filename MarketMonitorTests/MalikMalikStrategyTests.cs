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

namespace MarketMonitorTests
{
    public class MalikMalikStrategyTests
    {
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly MalikMalikStrategy _malikMalikStrategy;
        public MalikMalikStrategyTests()
        {
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _malikMalikStrategy = new MalikMalikStrategy(_mockHtmlWebAdapter.Object);
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
        public void ShouldReturnCorrectLastPageNumber_WhenHtmlIsValid(string html, int pageNumber)
        {
            //Arrange
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _malikMalikStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "url");

            //Assert
            Assert.Equal(pageNumber, result);
        }


    }
}
