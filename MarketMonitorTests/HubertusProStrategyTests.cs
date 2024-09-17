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
    public class HubertusProStrategyTests
    {
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly HubertusProStrategy _hubertusProStrategy;
        public HubertusProStrategyTests()
        {
            _htmlDocument = new HtmlDocument();
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _hubertusProStrategy = new HubertusProStrategy(_mockHtmlWebAdapter.Object);
        }

        public static IEnumerable<object[]> GetLastPageNumberTestCases()
        {
            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='pagination'>
                            <li><a href='#'><<</a></li>
                            <li><a href='#'>1</a></li>
                            <li><a href='/category/bron-bron-kulowa-sztucery-mysliwskie/2'>2</a></li>
                            <li><a href='/category/bron-bron-kulowa-sztucery-mysliwskie/3'>3</a></li>
                            <li><a href='/category/bron-bron-kulowa-sztucery-mysliwskie/2'>>></a></li>
                            <li><a href='/category/bron-bron-kulowa-sztucery-mysliwskie/3'>>>|</a></li>
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
                            <li><a href='#'>1</a></li>
                            <li><a href='/category/bron-bron-kulowa-sztucery-mysliwskie/1>>></a></li>
                            <li><a href='/category/bron-bron-kulowa-sztucery-mysliwskie/1>>>|</a></li>
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
        public void GetLastPageNumber_ShouldReturnCorrectLastPageNumber_WhenHtmlIsValid(string html, int pageNumber)
        {
            //Arrange
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _hubertusProStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "url");

            //Assert
            Assert.Equal(pageNumber, result);
        }

        [Fact]
        public void GetProducts_ShouldReturnCorrectProducts()
        {
            //Arrange
            var html = @"
            <html>
                <body>
                    <div class='product'>
                       <div class='image'>
                          <a href='/manufacturer/steyr-mannlicher/sztucer-impulse-elite-precision.html'></a>
                       </div>
                       <div class='product_name'>Sztucer Impulse Elite Precision</div>
                       <div class='price'>35553 zł</div>
                    </div>
                    <div class='product'>
                       <div class='image'>
                          <a href='/manufacturer/steyr-mannlicher/sztucer-impulse-elite-precision.html'></a>
                       </div>
                       <div class='product_name'>Sztucer Impulse Elite Precision</div>
                       <div class='price'>35553 zł</div>
                    </div>
                    <div class='product'>
                       <div class='image'>
                          <a href='/manufacturer/steyr-mannlicher/regulator-gazowy-steyr-aug-1252010460.html'></a>
                       </div>
                       <div class='product_name'>Regulator Gazowy Steyr AUG 1252010460</div>
                       <div class='price'>55,00 zł/szt</div>
                    </div>
                </body>
            </html>";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _hubertusProStrategy.GetProducts("url", 1);

            //Arrange
            List<Product> products = new List<Product>(result);

            Assert.Equal(2, products.Count);
            Assert.Equal("steyr-mannlicher/sztucer-impulse-elite-precision", products[0].IdProduct);
            Assert.Equal("Sztucer Impulse Elite Precision", products[0].Name);
            Assert.Equal(35553.00m, products[0].Price);
            Assert.Equal("steyr-mannlicher/regulator-gazowy-steyr-aug-1252010460", products[1].IdProduct);
            Assert.Equal("Regulator Gazowy Steyr AUG 1252010460", products[1].Name);
            Assert.Equal(55.00m, products[1].Price);
        }

        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            string html =
            @"
            <html>
                <div class='product'>
                    <div class='image'>
                        <a href='/manufacturer/steyr-mannlicher/regulator-gazowy-steyr-aug-1252010460.html'></a>
                    </div>
                    <div class='product_name'></div>
                    <div class='price'>55,00 zł/szt</div>
                </div>
            </html>
            ";

            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => { _hubertusProStrategy.GetProducts("url", 1); };

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetProducts_ShouldReturnEmptyList_WhenNoProductsInHtml()
        {
            // Arrange
            var html = "<html><body></body></html>";
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            // Act
            var result = _hubertusProStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
            Assert.NotNull(result);
        }
    }
}
