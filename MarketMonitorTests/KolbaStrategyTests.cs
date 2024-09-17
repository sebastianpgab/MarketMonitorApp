using FluentAssertions;
using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services;
using MarketMonitorApp.Services.ProductPatterns;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Net.WebRequestMethods;

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

        public static IEnumerable<object[]> ProductSearchTestData()
        {
            yield return new object[]
            {
                @"
                 <html>
                   <body>
                     <div class='product__name'> 
                        <a href='/product/53101,kamera-termowizyjna-termowizor-hikmicro-by-hikvision-falcon-fq50'>termowizor hikmicro by hikvision falcon fq50</a>
                     </div>
                     <div class='product__price'>
                         <div class='flex'>
                             <div class='flex'>
                                 <div class='text-xl'>23 zł</div>
                             </div>
                         </div>
                     </div>
                   </body>
                 </html>
                ",
                "https://kolba.pl/pl/search?page=1&sort=default&query=hikmicro", 
                5
            };
            yield return new object[]
            {
                @"
                 <html>
                   <body>
                     <div class='product__name'> 
                        <a href='/product/53102,puszka-real-hunter-300ml'>Puszka Real Hunter 300ml</a>
                     </div>
                     <div class='product__price'>
                         <div class='flex'>
                             <div class='flex'>
                                 <div class='text-xl'></div>
                             </div>
                         </div>
                     </div>
                   </body>
                 </html>
                ",
                "https://kolba.pl/pl/search?page=1&sort=default&query=hikmicro",
                9
            };
            yield return new object[]
            {
                @"
                 <html>
                   <body>
                     <div class='product__name'> 
                        <a href='/product/123,kamera-termowizyjna-termowizor-hikmicro-by-hikvision-falcon-fq50'>Kamera termowizyjna Falcon Fq50</a>
                     </div>
                     <div class='product__price'>
                         <div class='flex'>
                             <div class='flex'>
                                 <div class='text-xl'></div>
                             </div>
                         </div>
                     </div>
                   </body>
                 </html>
                ",
                "https://kolba.pl/pl/search?page=1&sort=default&query=hikmicro",
                3
            };
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


        [Theory]
        [MemberData(nameof(ProductSearchTestData))]
        public void GetProducts_WhenCalled_ShouldUpdatePageNumberInUrl(string html, string url, int currentPage)
        {
            string expectedUrl = null;
            _htmlDocument.LoadHtml(html);

            _htmlWebAdapter.Setup(p => p.Load(It.IsAny<string>()))
                           .Callback<string>(capturedUrl => expectedUrl = capturedUrl)
                           .Returns(_htmlDocument);

            _kolbaStrategy.GetProducts(url, currentPage);

            Assert.Equal("https://kolba.pl/pl/search?page="+currentPage+"&sort=default&query=hikmicro", expectedUrl);
        }

        [Theory]
        [MemberData(nameof(ProductSearchTestData))]
        public void GetProducts_ShouldReturnCorrectProducts(string html, string url, int currentPage)
        {
            _htmlDocument.LoadHtml(html);
            _htmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _kolbaStrategy.GetProducts(url, currentPage).ToList();

            // Assert
            if (currentPage == 1)
            {
                Assert.Single(result);
                Assert.Equal("53101", result[0].IdProduct);
                Assert.Equal("termowizor hikmicro by hikvision falcon fq50", result[0].Name);
                Assert.Equal(23.00m, result[0].Price);
            }
            else if (currentPage == 9)
            {
                Assert.Single(result);
                Assert.Equal("53102", result[0].IdProduct);
                Assert.Equal("Puszka Real Hunter 300ml", result[0].Name);
                Assert.Equal(0m, result[0].Price);
            }
            else if (currentPage == 3)
            {
                Assert.Single(result);
                Assert.Equal("123", result[0].IdProduct);
                Assert.Equal("Kamera termowizyjna Falcon Fq50", result[0].Name);
                Assert.Equal(0m, result[0].Price);
            }
        }


        [Fact]
        public void GetProducts_MissingProductNameSelector_ShouldThrowNullReferenceException()
        {
            string html =
            @"
                <html>
                <body>
                    <div class='product__name'> 
                    <a href='/product/123,kamera-termowizyjna-termowizor-hikmicro-by-hikvision-falcon-fq50'></a>
                    </div>
                    <div class='product__price'>
                        <div class='flex'>
                            <div class='flex'>
                                <div class='text-xl'></div>
                            </div>
                        </div>
                    </div>
                </body>
                </html>
            ";

            _htmlDocument.LoadHtml(html);
            _htmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            Action action = () => { _kolbaStrategy.GetProducts("url", 1); };

            //Assert
            Assert.Throws<ArgumentException>(action);
        }


        [Fact]
        public void GetProducts_ShouldReturnEmptyList_WhenNoProductsInHtml()
        {
            // Arrange
            var html = "<html><body></body></html>";
            _htmlDocument.LoadHtml(html);
            _htmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            // Act
            var result = _kolbaStrategy.GetProducts("url", 1);

            // Assert
            Assert.Empty(result);
            Assert.NotNull(result);
        }


    }
}
