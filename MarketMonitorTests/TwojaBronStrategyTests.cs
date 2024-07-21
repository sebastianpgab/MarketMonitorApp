using Moq;
using Xunit;
using System.Collections.Generic;
using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;
using MarketMonitorApp.Services;
using System;
using System.Runtime.Serialization;

public class TwojaBronStrategyTests
{
    Mock<IHtmlWebAdapter> _mockHtmlWebAdapter;

    public TwojaBronStrategyTests()
    {
        _mockHtmlWebAdapter = new Mock<IHtmlWebAdapter>();
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

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        _mockHtmlWebAdapter.Setup(web => web.Load(It.IsAny<string>())).Returns(htmlDoc);

        var strategy = new TwojaBronStrategy(_mockHtmlWebAdapter.Object);

        // Act
        var result = strategy.GetProducts("http://example.com", 1);

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
}

