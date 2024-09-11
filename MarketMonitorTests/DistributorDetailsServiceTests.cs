using MarketMonitorApp.Entities;
using MarketMonitorApp.Services;
using Microsoft.EntityFrameworkCore;
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
        private readonly Mock<DistributorDetailsService> _distributorDetailsServiceMocked;

        public DistributorDetailsServiceTests()
        {
            _marketMonitorDbContext = new Mock<MarketMonitorDbContext>();
            _distributorDetailsService = new DistributorDetailsService(_marketMonitorDbContext.Object);
            _distributorDetailsServiceMocked = new Mock<DistributorDetailsService>(_marketMonitorDbContext.Object);
        }

        [Fact]
        public void IsProductDeleted_WhenProductIsMarkedAsDeleted_ShouldSetIsDeletedAndUnsetIsNew()
        {
            //Arrange
            List<Product> oldProducts = new List<Product>() { new Product { Id = 1, IdProduct = "1", IsNew = true, IsDeleted = false } };
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

        [Fact]
        public void CompareProducts_WhenNewProductIsAdded_ShouldReturnAllNewProducts()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "TestCategory" };

            var actualization = new Actualization
            {
                Products = new List<Product>
            {
                new Product { IdProduct = "1", Price = 100 },
                new Product { IdProduct = "2", Price = 200 },
                new Product { IdProduct = "3", Price = 300 },
                new Product { IdProduct = "4", Price = 300 }

            }
            };

            var lastUpdatedProducts = new List<Product>
            {
            new Product { IdProduct = "1", Price = 100 },
            new Product { IdProduct = "2", Price = 200 }
            };

            _distributorDetailsServiceMocked
                .Setup(pc => pc.LastUpdatedProducts(It.IsAny<Actualization>(), It.IsAny<Category>()))
                .Returns(lastUpdatedProducts);

            // Act
            var result = _distributorDetailsServiceMocked.Object.CompareProducts(actualization, category);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("3", result[0].IdProduct);
            Assert.False(result[0].IsDeleted);
            Assert.True(result[0].IsNew);

            Assert.Equal("4", result[1].IdProduct);
            Assert.False(result[1].IsDeleted);
            Assert.True(result[1].IsNew);


        }

        [Fact]
        public void CompareProducts_WhenNewProductIsDeleted_ShouldReturnAllDeletedProducts()
        {
            //Arrange
            var category = new Category { Id = 1, Name = "TestCategory" };

            var actualization = new Actualization
            {
                Products = new List<Product>
            {
                new Product { IdProduct = "1", Price = 100 },
                new Product { IdProduct = "2", Price = 200 },

            }
            };

            var lastUpdatedProducts = new List<Product>
            {
                new Product { IdProduct = "1", Price = 100 },
                new Product { IdProduct = "2", Price = 200 },
                new Product { IdProduct = "3", Price = 300 },
                new Product { IdProduct = "4", Price = 300 }
            };

            _distributorDetailsServiceMocked.Setup(p => p.LastUpdatedProducts(It.IsAny<Actualization>(), It.IsAny<Category>())).Returns(lastUpdatedProducts);

            //Act
            var result = _distributorDetailsServiceMocked.Object.CompareProducts(actualization, category);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal("3", result[0].IdProduct);
            Assert.True(result[0].IsDeleted);

            Assert.Equal("4", result[1].IdProduct);
            Assert.True(result[1].IsDeleted);
        }

        [Fact]
        public void CompareProducts_WhenProductPriceIsChanged_ShouldReturnUpdatedProducts()
        {
            //Arrange
            var category = new Category { Id = 1, Name = "TestCategory" };

            var actualization = new Actualization
            {
                Products = new List<Product>
            {
                new Product { IdProduct = "1", Price = 100 },
                new Product { IdProduct = "2", Price = 200 },

            }
            };

            var lastUpdatedProducts = new List<Product>
            {
                new Product { IdProduct = "1", Price = 199 },
                new Product { IdProduct = "2", Price = 450 },
            };

            _distributorDetailsServiceMocked.Setup(p => p.LastUpdatedProducts(It.IsAny<Actualization>(), It.IsAny<Category>())).Returns(lastUpdatedProducts);

            //Act
            var result = _distributorDetailsServiceMocked.Object.CompareProducts(actualization, category);

            //Assert
            Assert.NotEmpty(result);
            Assert.False(result[0].IsDeleted);
            Assert.False(result[0].IsNew);
            Assert.Equal(100m, result[0].Price);

            Assert.False(result[1].IsDeleted);
            Assert.False(result[1].IsNew);
            Assert.Equal(200m, result[1].Price);
        }

        [Fact]
        public void CompareProducts_WhenNoChanges_ShouldReturnEmptyList()
        {
            //Arrange
            var category = new Category { Id = 1, Name = "TestCategory" };

            var actualization = new Actualization
            {
                Products = new List<Product> { }
            };

            var lastUpdatedProducts = new List<Product>
            {
                new Product { IdProduct = "1", Price = 199 },
                new Product { IdProduct = "2", Price = 450 },
            };

            _distributorDetailsServiceMocked.Setup(p => p.LastUpdatedProducts(It.IsAny<Actualization>(), It.IsAny<Category>())).Returns(lastUpdatedProducts);

            //Act
            var result = _distributorDetailsServiceMocked.Object.CompareProducts(actualization, category);

            //Assert
            Assert.Empty(result);
        }

    }
}