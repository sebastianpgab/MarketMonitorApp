using MarketMonitorApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class SeedData
{
    private readonly MarketMonitorDbContext _marketMonitorDbContext;

    public SeedData(MarketMonitorDbContext marketMonitorDbContext)
    {
        _marketMonitorDbContext = marketMonitorDbContext;
    }

    public void Initialize()
    {
        if (!_marketMonitorDbContext.Actualizations.Any())
        {
            var distributors = new List<Distributor>
            {
                new Distributor
                {
                    Name = "Twoja Bron",
                    Categories = new List<Category>
                    {
                        new Category 
                        { 
                        Name = "Akcesoria",
                        LinkToCategory = "https://twojabron.pl/kategoria/akcesoria"
                        },
                        new Category 
                        { Name = "Noktowizja",
                          LinkToCategory = "https://twojabron.pl/kategoria/noktowizja"
                        }
                    }
                },
                new Distributor
                {
                    Name = "Delta Optical",
                    Categories = new List<Category>
                    {
                        new Category 
                        {
                            Name = "Lornetki", 
                            LinkToCategory = "https://deltaoptical.pl/lornetki/page:"
                        },
                        new Category 
                        {
                           Name = "Termowizory",
                           LinkToCategory = "https://deltaoptical.pl/termowizory/page:"
                        }
                    }
                }
            };

            _marketMonitorDbContext.Distributors.AddRange(distributors);
            _marketMonitorDbContext.SaveChanges(); 

            var actualizations = new List<Actualization>
            {
                new Actualization
                {
                    IsEntered = true,
                    DistributorId = distributors[0].Id, 
                    Products = new List<Product>
                    {
                        new Product { Name = "Sztucer Blaser R8", IdProduct = "001", Price = 665},
                        new Product { Name = "Nóż Mora",  IdProduct = "002",  Price = 543 }
                    }
                },
                new Actualization
                {
                    IsEntered = true,
                    DistributorId = distributors[1].Id,  
                    Products = new List<Product>
                    {
                        new Product { Name = "Aschutz 1470 sztucer kal 30-06", IdProduct = "001", Price = 23323},
                        new Product { Name = "Słuchawki Browning", IdProduct = "002", Price = 123}
                    }
                }
            };

            _marketMonitorDbContext.Actualizations.AddRange(actualizations);
            _marketMonitorDbContext.SaveChanges(); 
        }
    }
}
