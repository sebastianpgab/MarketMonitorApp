using MarketMonitorApp.Entities;
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
    public class DistributorDetailsServiceTests
    {
        private readonly DistributorDetailsService _distributorDetailsService;
        private readonly Mock<MarketMonitorDbContext> _marketMonitorDbContext;
        public DistributorDetailsServiceTests()
        {
            _marketMonitorDbContext = new Mock<MarketMonitorDbContext>();
            _distributorDetailsService = new DistributorDetailsService(_marketMonitorDbContext.Object);
        }

        [Fact]
        public void IsProductDeleted_WhenProductIsMarkedAsDeleted_ShouldSetIsDeletedAndUnsetIsNew()
        {
            //Arrange
            List<Product> oldProducts = new List<Product>() { new Product { Id = 1, IdProduct = "1", IsNew = true, IsDeleted = false}};
            List<Product> newProducts = new List<Product>();
            List<Product> comparedProducts = new List<Product>();

            //Act
            _distributorDetailsService.IsProductDeleted(oldProducts, newProducts, comparedProducts);

            //Assert
            Assert.Single(comparedProducts);
            Assert.True(comparedProducts[0].IsDeleted);
            Assert.False(comparedProducts[0].IsNew);
        }

        [Fact]
        public void IsProductDeleted_WhenProductExistsInNewList_ShouldNotUpdateProduct()
        {
            //Arrange
            List<Product> oldProducts = new List<Product>() { new Product { Id = 1, IdProduct = "1", IsNew = false, IsDeleted = false } };
            List<Product> newProducts = new List<Product> { new Product { Id = 1, IdProduct = "1" } };
            List<Product> comparedProducts = new List<Product>();

            //Act
            _distributorDetailsService.IsProductDeleted(oldProducts, newProducts, comparedProducts);

            //Assert
            Assert.Empty(comparedProducts);
        }

    }
}
