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
    }
}
