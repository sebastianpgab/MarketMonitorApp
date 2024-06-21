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
                            LinkToCategory = "https://deltaoptical.pl/lornetki"
                        },
                        new Category
                        {
                           Name = "Termowizory",
                           LinkToCategory = "https://deltaoptical.pl/termowizory"
                        },
                        new Category
                        {
                           Name = "Lunety celownicze",
                           LinkToCategory = "https://deltaoptical.pl/lunety-celownicze"
                        },
                        new Category
                        {
                           Name = "Lunety obserwacyjne",
                           LinkToCategory = "https://deltaoptical.pl/lunety-obserwacyjne"
                        }
                    }
                },
                new Distributor
                {
                    Name = "Hubertus Białystok",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Ceska Zbrojovka",
                            LinkToCategory = "https://hubertus.com.pl/search/6,1,default-asc/text=ceska/pl.html"
                        }
                    }

                },
                new Distributor
                {
                    Name = "Szuster",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Blaser",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/blaser"
                        },
                        new Category
                        {
                            Name ="RWS",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/rws"
                        },
                        new Category
                        {
                            Name ="Mauser",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/mauser"
                        },
                        new Category
                        {
                            Name ="Sauer",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/sauer"
                        },
                        new Category
                        {
                            Name ="Sig-Sauer",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/sig-sauer"
                        },
                        new Category
                        {
                            Name ="Zeiss",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/zeiss"
                        },
                        new Category
                        {
                            Name ="Geco",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/geco"
                        },
                        new Category
                        {
                            Name ="Hausken",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/hausken"
                        },
                        new Category
                        {
                            Name ="Rottweill",
                            LinkToCategory = "https://sklep.szuster.com.pl/manufacturer/rottweill"
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
                            Name ="Browning",
                            LinkToCategory = "https://www.knieja.com.pl/marki/1-browning?page="
                        },
                        new Category
                        {
                            Name ="GRS",
                            LinkToCategory = "https://www.knieja.com.pl/marki/7-grs?page="
                        },
                        new Category
                        {
                            Name ="Guide",
                            LinkToCategory = "https://www.knieja.com.pl/marki/10-guide-sensmart?page="
                        },
                        new Category
                        {
                            Name ="Meopta",
                            LinkToCategory = "https://www.knieja.com.pl/marki/3-meopta?page="
                        },
                        new Category
                        {
                            Name ="Norma",
                            LinkToCategory = "https://www.knieja.com.pl/marki/4-norma?page="
                        },
                        new Category
                        {
                            Name ="Winchester",
                            LinkToCategory = "https://www.knieja.com.pl/marki/2-winchester?page="
                        },

                    },
                },
                 new Distributor
                {
                    Name = "Hubertus Pro",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Haenel",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/haenel/"
                        },
                        new Category
                        {
                            Name ="Nightforce",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/nightforce/"
                        },
                        new Category
                        {
                            Name ="Savage",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/savage/"
                        },
                        new Category
                        {
                            Name ="Merkel",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/merkel/"
                        },
                        new Category
                        {
                            Name ="Fabarm",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/fabarm/"
                        },
                        new Category
                        {
                            Name ="Aimpoint",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/aimpoint-ab/"
                        },
                        new Category
                        {
                            Name ="Hornady",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/hornady/"
                        },
                        new Category
                        {
                            Name ="Steyr-Mannlicher",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/steyr-mannlicher/"
                        },
                        new Category
                        {
                            Name ="Leica",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/leica-camera-ag/"
                        },
                        new Category
                        {
                            Name ="Recknagel",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/recknagel/"
                        },
                        new Category
                        {
                            Name ="Haenel",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/haenel/"
                        },
                        new Category
                        {
                            Name ="Niggeloh",
                            LinkToCategory = "https://sklep.hubertusprohunting.pl/manufacturer/niggeloh/"
                        },

                    },

                },
                 new Distributor
                {
                    Name = "Kaliber",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Tikka",
                            LinkToCategory = "https://kaliber.pl/39_tikka?p="
                        },
                        new Category
                        {
                            Name ="Steiner",
                            LinkToCategory = "https://kaliber.pl/80_steiner?p="
                        },
                        new Category
                        {
                            Name ="Smithwesson",
                            LinkToCategory = "https://kaliber.pl/2_smithwesson?p="
                        },
                        new Category
                        {
                            Name ="Sako",
                            LinkToCategory = "https://kaliber.pl/10_sako?p="
                        },
                        new Category
                        {
                            Name ="Glock",
                            LinkToCategory = "https://kaliber.pl/68_glock?p="
                        },
                        new Category
                        {
                            Name ="Beretta",
                            LinkToCategory = "https://kaliber.pl/5_beretta?p="
                        },

                    },
                },
                 new Distributor
                {
                    Name = "Incorsa",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Anschutz",
                            LinkToCategory = "https://sklep.incorsa.pl/anschutz?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Ase-Utra",
                            LinkToCategory = "https://sklep.incorsa.pl/ase-utra?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Ballistol",
                            LinkToCategory = "https://sklep.incorsa.pl/ballistol?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Benelli",
                            LinkToCategory = "https://sklep.incorsa.pl/benelli?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Berger",
                            LinkToCategory = "https://sklep.incorsa.pl/berger?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Bergara",
                            LinkToCategory = "https://sklep.incorsa.pl/bergara?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Fair",
                            LinkToCategory = "https://sklep.incorsa.pl/fair?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Lapua Gmbh",
                            LinkToCategory = "https://sklep.incorsa.pl/lapua-gmbh?page=1&elems_per_page=10"
                        },
                        new Category
                        {
                            Name ="Lapua Nammo",
                            LinkToCategory = "https://sklep.incorsa.pl/nammo-lapua?page=1&elems_per_page=10"
                        }
                    }

                },
                 new Distributor
                {
                    Name = "Malik Malik",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="ATA Arms",
                            LinkToCategory = "https://malik-malik.pl/producenci/produkty-ata-arms?start="
                        },
                        new Category
                        {
                            Name ="Sabatti",
                            LinkToCategory = "https://malik-malik.pl/producenci/produkty-sabatti?start="
                        },
                        new Category
                        {
                            Name ="Retay",
                            LinkToCategory = "https://malik-malik.pl/producenci/produkty-retay?start="
                        }
                    }
                },
                 new Distributor
                {
                    Name = "RParms",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Ruger",
                            LinkToCategory = "https://rparms.pl/szukaj?controller=search&poscats=0&s=Ruger&page="
                        },

                    }
                },
                  new Distributor
                {
                    Name = "Kolba",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Hikmikro",
                            LinkToCategory = "https://kolba.pl/pl/search?page=1&sort=default&query=hikmicro"
                        },
                        new Category
                        {
                            Name ="RealHunter",
                            LinkToCategory = "https://kolba.pl/pl/search?page=1&sort=default&query=realhunter"
                        },
                        new Category
                        {
                            Name ="Primos",
                            LinkToCategory = "https://kolba.pl/pl/search?page=1&sort=default&query=primos"
                        },
                        new Category
                        {
                            Name ="Vortex",
                            LinkToCategory = "https://kolba.pl/pl/search?page=1&sort=default&query=vortex"
                        },
                    }
                },
                 new Distributor
                {
                    Name = "TaniePolowanie",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="PARD",
                            LinkToCategory = "https://taniepolowanie.pl/pl/searchquery/pard/"
                        },
                        new Category
                        {
                            Name ="Senopex",
                            LinkToCategory = "https://taniepolowanie.pl/pl/searchquery/senopex/"
                        },
                    }
                },
                 new Distributor
                {
                    Name = "Tamed",
                    Categories = new List<Category>
                    {
                        new Category
                        {
                            Name ="Infiray",
                            LinkToCategory = "https://tamed.pl/index.php?fc=module&module=leoproductsearch&controller=productsearch&leoproductsearch_static_token=66cc3dc6468290af5fe3ebc958502f6b&search_query=infiray&page="
                        },
                        new Category
                        {
                            Name ="X-Hog",
                            LinkToCategory = "https://tamed.pl/index.php?fc=module&module=leoproductsearch&controller=productsearch&leoproductsearch_static_token=66cc3dc6468290af5fe3ebc958502f6b&cate=&search_query=x-hog+iluminator&page="
                        },
                    }
                },

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
