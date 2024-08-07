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

        [Fact]
        public void GetProducts_ShouldReturnCorrectProducts()
        {
            //Arrange
            string html = @"
            <html>
              <div class='djc_item_in'>
                <div class='djc_price' data-itemid=123></div>
                <div class='djc_title'>Ata Arms karabin 30-06</div>
                <div class='djc_price_value'>24,00 zł</div> 
              </div>
              <div class='djc_item_in'>
                <div class='djc_price' data-itemid=123></div>
                <div class='djc_title'>Ata Arms karabin 30-06</div>
                <div class='djc_price_value'>24,00 zł</div> 
              </div>
              <div class='djc_item_in'>
                <div class='djc_price' data-itemid=1234></div>
                <div class='djc_title'>Blaser R8 Ultimate Carbon</div>
                <div class='djc_price_value'>222324,00 zł</div> 
              </div>
            </html>";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _malikMalikStrategy.GetProducts("url", 1);

            List<Product> products = new List<Product>(result);

            //Assert
            Assert.Equal(2, products.Count());
            Assert.Equal("123", products[0].IdProduct);
            Assert.Equal("Ata Arms karabin 30-06", products[0].Name);
            Assert.Equal(24.00m, products[0].Price);
            Assert.Equal("1234", products[1].IdProduct);
            Assert.Equal("Blaser R8 Ultimate Carbon", products[1].Name);
            Assert.Equal(222324.00m, products[1].Price);
        }

        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            string html =
            @"
            <html>
              <div class='djc_item_in'>
                <div class='djc_price' data-itemid=123></div>
                <div class='djc_price_value'>222324,00 zł</div> 
              </div>
            </html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => { _malikMalikStrategy.GetProducts("url", 1); };

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
            var result = _malikMalikStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
            Assert.NotNull(result);
        }


    }
}
