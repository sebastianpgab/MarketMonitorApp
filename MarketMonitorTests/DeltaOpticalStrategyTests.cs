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
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using FluentAssertions.Common;
using Microsoft.Extensions.Options;

namespace MarketMonitorTests
{
    public class DeltaOpticalStrategyTests
    {
        private readonly HtmlDocument _htmlDocument;
        private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
        private readonly DeltaOpticalStrategy _deltaOpticalStrategy;

        public DeltaOpticalStrategyTests()
        {
            _htmlDocument = new HtmlDocument();
            _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
            _deltaOpticalStrategy = new DeltaOpticalStrategy(_mockHtmlWebAdapter.Object);
        }

        public static IEnumerable<object[]> GetLastPageNumberTestCases()
        {

            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='paginator-info'>(140)</div>
                    </body>
                </html>
                ",
                12
            };

            yield return new object[]
            {
                @"
                <html>
                    <body>
                        <div class='paginator-info'></div>
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
                        <div class='paginator-info'>(abc)</div>
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
                        <div class='paginator-info'>(5)</div>
                     </body>
                </html>
                ",
                1
            };
        }


        [Theory]
        [MemberData (nameof(GetLastPageNumberTestCases))]
        public void GetLastPageNumber_ShouldReturnCorrectLastPageNumber_WhenHtmlIsValid(string html, int pageNumber)
        {
            //Arrange
            _htmlDocument.LoadHtml(html);
            _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

            //Act
            var result = _deltaOpticalStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, "");

            //Assert
            Assert.Equal(pageNumber, result);
        }

        //[Fact]
        //public void GetProducts_ShouldReturnCorrectProducts()
        //{
        //    var html =
        //    @"
        //     <html>
        //       <body>
        //         <div class='product-box' data-product-id='123'>
        //           <h2>
        //             <div class='product-name'><a title='Delta Optical Lornetka 12x50'></a></div>>
        //           </h2>
        //           <div class='price'>123 zł</div>
        //         </div>
        //       </body>
        //     </html>
        //    ";
        //}


    }
}
