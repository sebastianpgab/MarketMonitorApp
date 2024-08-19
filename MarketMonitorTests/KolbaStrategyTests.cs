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
                "https://kolba.pl/pl/search?page=1&sort=default&query=realhunter",
                2,
                @"
                 <html>
                   <body>
                     <div class='product__name'> 
                        <a href='/product/53101,kamera-termowizyjna-termowizor-hikmicro-by-hikvision-falcon-fq50'>Puszka Real Hunter 300ml</a>
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
                "
            };
            yield return new object[]
{
                "https://kolba.pl/pl/search?page=1&sort=default&query=realhunter",
                4,
                @"
                 <html>
                   <body>
                     <div class='product__name'> 
                        <a href='/product/53101,kamera-termowizyjna-termowizor-hikmicro-by-hikvision-falcon-fq50'>Bron 123</a>
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
                "
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
        public void GetProducts_ShouldReturnCorrectProducts(string url, int currentPage, string html)
        {
            _htmlDocument.LoadHtml(html);

            _htmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            var result = _kolbaStrategy.GetProducts(url, currentPage);

            List<Product> products = new List<Product>(result);


            if(currentPage == 2)
            {
                Assert.Equal(products[0].Name, "Puszka Real Hunter 300ml");
            }
            else if(currentPage == 4) 
            {
                Assert.Equal(products[0].Name, "Bron 123");
            }

        }
    }
}
