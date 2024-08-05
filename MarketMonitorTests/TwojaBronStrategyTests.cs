using Moq;
using Xunit;
using System.Collections.Generic;
using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using MarketMonitorApp.Services;
using System;
using MarketMonitorApp.Services.ProductPatterns;

public class TwojaBronStrategyTests
{
    private readonly Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;
    private readonly IDistributorStrategy _twojaBronStrategy;
    private readonly HtmlDocument _htmlDocument;

    public TwojaBronStrategyTests()
    {
        _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
        _twojaBronStrategy = new TwojaBronStrategy(_mockHtmlWebAdapter.Object);
        _htmlDocument = new HtmlDocument();
    }

    public static IEnumerable<object[]> GetPaginatorList()
    {
        yield return new object[]
        {
            @"
            <html>
                <body>
                    <div class='paginator'>
                        <li><a>1</a></li>
                        <li><a>2</a></li>
                        <li><a>3</a></li>
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
                    <div class='paginator'>
                        <li> </li>
                    </div>
                </body>
            </html>
            ",
            1
        };
    }

    [Fact]
    public void GetProducts_ShouldReturnExpectedProducts()
    {
        // Arrange
        var html = @"
            <html>
                <body>
                    <div class='product' data-product-id='123'>
                        <div class='prodname'>Product 1</div>
                        <div class='price'><em>10,00 zł</em></div>
                    </div>
                    <div class='product' data-product-id='1234'>
                        <div class='prodname'>Product 2</div>
                        <div class='price'><em>11,00 zł</em></div>
                    </div>
                </body>
            </html>";

        _htmlDocument.LoadHtml(html);
        _mockHtmlWebAdapter.Setup(web => web.Load(It.IsAny<string>())).Returns(_htmlDocument);

        // Act
        var result = _twojaBronStrategy.GetProducts(It.IsAny<string>(), 1);

        // Assert
        Assert.NotNull(result);
        var productList = new List<Product>(result);
        Assert.Equal(2, productList.Count);
        Assert.Equal("123", productList[0].IdProduct);
        Assert.Equal("Product 1", productList[0].Name);
        Assert.Equal(10.00m, productList[0].Price);
        Assert.Equal("1234", productList[1].IdProduct);
        Assert.Equal("Product 2", productList[1].Name);
        Assert.Equal(11.00m, productList[1].Price);
    }

    [Theory]
    [MemberData(nameof(GetPaginatorList))]
    public void GetLastPageNumber_ShouldReturnExpectedLastPage(string html, int numberOfPage)
    {
        // Arrange
        _htmlDocument.LoadHtml(html);
        _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

        // Act
        var result = _twojaBronStrategy.GetLastPageNumber(_mockHtmlWebAdapter.Object, It.IsAny<string>());

        // Assert
        Assert.Equal(numberOfPage, result);
    }

    [Theory]
    [InlineData("321,00 zł", 321.00)]
    [InlineData("321,00zł", 321.00)]
    [InlineData("321.00zł", 321.00)]
    [InlineData(" 321,00 zł ", 321.00)]
    [InlineData("321 zł", 321.00)]
    [InlineData("321", 321.00)]
    [InlineData(null, 0)]
    [InlineData("", 0)]

    public void CleanPrice_ValidPriceString_ShouldReturnExpectedDecimal(string price , decimal expected)
    {
        //Act
        var result = _twojaBronStrategy.CleanPrice(price);

        //Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("54..00zł")]
    [InlineData("54.,00zł")]
    [InlineData("00,00 brutto")]
    public void CleanPrice_InvalidPriceString_ShouldReturnExpectedDecimal(string price)
    {
        //Act
        Action action = () => { _twojaBronStrategy.CleanPrice(price);};

        //Assert
        Assert.Throws<FormatException>(action);
    }

    [Fact]
    public void GetProducts_ShouldReturnEmptyList_WhenNoProductsInHtml()
    {
        var html =
        @"
             <html>
                 <div class='no-products'></div>
             </html>
            ";

        _htmlDocument.LoadHtml(html);
        _mockHtmlWebAdapter.Setup(p => p.Load(It.IsAny<string>())).Returns(_htmlDocument);

        var result = _twojaBronStrategy.GetProducts("url", 1);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}