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
    public class SzusterStrategyTests
    {
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly SzusterStrategy _szusterStrategy;
        private readonly HtmlDocument _htmlDocument;

        public SzusterStrategyTests()
        {
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _szusterStrategy = new SzusterStrategy(_mockHtmlWebAdapter.Object);
            _htmlDocument = new HtmlDocument();
        }

        private static IEnumerable<object[]> HtmlPaginationTestData()
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
        [MemberData(nameof(HtmlPaginationTestData))]
        public void GetLastPageNumber_ShouldReturnCorrectLastPageNumber_WhenHtmlIsValid(string htmlContent, int expectedLastPageNumber)
        {
            // Arrange
            _htmlDocument.LoadHtml(htmlContent);
            _mockHtmlWebAdapter.Setup(adapter => adapter.Load(It.IsAny<string>())).Returns(_htmlDocument);

            // Act
            var actualLastPageNumber = _szusterStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "url");

            // Assert
            Assert.Equal(expectedLastPageNumber, actualLastPageNumber);
        }

        [Fact]
        public void GetProducts_ShouldReturnCorrectProducts()
        {
            var html =
            @"
             <html>
                 <div class='product'>
                     <div class='name'><a href='/manufacturer/rws/srut-rws-hobby-5-5-mm-0-77g-500-srucin.html'></a></div>
                     <div class='price'>45,00 zł</div>
                 </div>
                 <div class='product'>
                     <div class='name'><a href='/manufacturer/jsb/srut-jsb-exact-4-5-mm-0-547g-500-sztuk.html'></a></div>
                     <div class='price'>70,00 zł</div>
                 </div>
                 <div class='product'>
                     <div class='name'><a href='/manufacturer/jsb/srut-jsb-exact-4-5-mm-0-547g-500-sztuk.html'></a></div>
                     <div class='price'>70,00 zł</div>
                 </div>
             </html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            var result = _szusterStrategy.GetProducts("url", 1);

            List<Product> products = new List<Product>(result);

            Assert.NotNull(result);
            Assert.Equal(2, products.Count);
            Assert.Equal("srut rws hobby 5 5 mm 0 77g 500 srucin", products[0].Name);
            Assert.Equal("rws/srut-rws-hobby-5-5-mm-0-77g-500-srucin", products[0].IdProduct);
            Assert.Equal(45.00m, products[0].Price);

            Assert.Equal("srut jsb exact 4 5 mm 0 547g 500 sztuk", products[1].Name);
            Assert.Equal("jsb/srut-jsb-exact-4-5-mm-0-547g-500-sztuk", products[1].IdProduct);
            Assert.Equal(70.00m, products[1].Price);
        }

        [Fact]
        public void GetProducts_ShouldReturnEmptyList_WhenNoProductsInHtml()
        {
            var html =
            @"
              <html><body></body></html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            var result = _szusterStrategy.GetProducts("url", 1);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            string html =
            @"
            <html>
              <div class='product'>
                <div class='price'>45,00 zł</div>
              </div>
            </html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => { _szusterStrategy.GetProducts("url", 1); };

            //Assert
            Assert.Throws<NullReferenceException>(action);
        }

    }
}
