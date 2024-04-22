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
                    Name = "Knieja",
                    Categories = new List<Category>
                    {
                        new Category 
                        {
                            Name = "Ubrania TB", 
                            LinkToCategory = "https://twojabron.pl/kategoria/bron-nowa" 
                        },
                        new Category 
                        {
                           Name = "Ochorna słuchu TB",
                           LinkToCategory = "https://twojabron.pl/kategoria/bron-nowa"
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
                        new Product { Name = "Sztucer Blaser R8", IdProduct = "001", Price = "2400"},
                        new Product { Name = "Nóż Mora",  IdProduct = "002",  Price = "23231" }
                    }
                },
                new Actualization
                {
                    IsEntered = true,
                    DistributorId = distributors[1].Id,  
                    Products = new List<Product>
                    {
                        new Product { Name = "Aschutz 1470 sztucer kal 30-06", IdProduct = "001", Price = "32322"},
                        new Product { Name = "Słuchawki Browning", IdProduct = "002", Price = "4343"}
                    }
                }
            };

            _marketMonitorDbContext.Actualizations.AddRange(actualizations);
            _marketMonitorDbContext.SaveChanges(); 
        }
    }
}
