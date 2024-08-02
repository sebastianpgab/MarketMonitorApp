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
    public class SzusterStrategyTests
    {
        private readonly Mock<IHtmlWebAdapter> _htmlWebAdapter;
        private readonly SzusterStrategy _szusterStrategy;
        private readonly HtmlDocument _htmlDocument;
        public SzusterStrategyTests()
        {
            _htmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _szusterStrategy = new SzusterStrategy(_htmlWebAdapter.Object);
            _htmlDocument = new HtmlDocument();
        }

        private static IEnumerable<object[]> GetData()
        {
            yield return new object[]
            {
            @"<html>
              <div class='pagination'>
              <li><a href='/manufacturer/rws/8'></a></li>
              </div>
            </html>",
            8
            };
            yield return new object[]
            {
            @"<html>
              <div class='pagination'>
              <li><a href='/manufacturer/rws/'></a></li>
              </div>
            </html>",
            1
            };
            yield return new object[]
            {
            @"<html>
              <div class='pagination'>
              <li><a href=''></a></li>
              </div>
            </html>",
            1
            };
            yield return new object[]
            {
            @"<html>
              <div class=''>
              <li><a href=''></a></li>
              </div>
            </html>",
            1
            };
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void GetLastPageNumber_WhenHtmlIsValid_ShouldReturnCorrectLastPageNumber(string html, int lastPage)
        {
            //Arrange
            _htmlDocument.LoadHtml(html);
            _htmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _szusterStrategy.GetLastPageNumber(_htmlWebAdapter.Object, "ulr");

            //Arrange
            Assert.Equal(lastPage, result);
        }
    }
}
