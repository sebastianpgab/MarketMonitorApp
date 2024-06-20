using MarketMonitorApp;
using MarketMonitorApp.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;
using Xunit;
using Moq;
using static System.Net.WebRequestMethods;

namespace MarketMonitorTests
{
    public class PriceScraperTests
    {

        [Fact]
        public void GetProducts_ShouldReturnAllProducts_FromAllPages()
        {
            //arrange
            var distributor = new Distributor();
            distributor.Id = 1;
            distributor.Name = "Test";
            distributor.Categories = new List<Category>() 
            { 
                new Category { 
                    Id = 1, 
                    Name = "Test Category",  
                    LinkToCategory = "Test Link", 
                    DistributorId = distributor.Id, 
                    Distributor = distributor 
                } 
            };


        }
    }
}