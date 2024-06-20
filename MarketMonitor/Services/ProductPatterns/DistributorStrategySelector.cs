using HtmlAgilityPack;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public interface IDistributorStrategySelector
    {
        public IDistributorStrategy ChoseStrategy(Distributor distributor);
    }
    public class DistributorStrategySelector : IDistributorStrategySelector
    {
        private readonly HtmlWeb _htmlWeb;
        public DistributorStrategySelector(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
        public IDistributorStrategy ChoseStrategy(Distributor distributor)
        {
            switch (distributor.Name)
            {
                case ("Twoja Bron"):
                    {
                        return new TwojaBronStrategy(_htmlWeb);
                    }
                case ("Hubertus Białystok"):
                    {
                        return new HubertusBialystokStrategy(_htmlWeb);
                    }
                case ("Delta Optical"):
                    {
                        return new DeltaOpticalStrategy();
                    }
                case ("Szuster"):
                    {
                        return new SzusterStrategy(_htmlWeb);
                    }
                case ("Knieja"):
                    {
                        return new KniejaStrategy(_htmlWeb);
                    }
                case ("Hubertus Pro"):
                    {
                        return new HubertusProStrategy(_htmlWeb);
                    }
                case ("Kaliber"):
                    {
                        return new KaliberStrategy(_htmlWeb);
                    } 
                case ("Incorsa"):
                    {
                        return new IncorsaStrategy(_htmlWeb);
                    }
                case ("Malik Malik"):
                    {
                        return new MalikMalikStrategy(_htmlWeb);
                    }
                case ("RParms"):
                    {
                        return new RParmsStrategy(_htmlWeb);
                    }
                case ("Kolba"):
                    {
                        return new KolbaStrategy(_htmlWeb);
                    }
                case ("TaniePolowanie"):
                    {
                        return new TaniePolowanieStrategy(_htmlWeb);
                    }
                case ("Tamed"):
                    {
                        return new TamedStrategy(_htmlWeb);
                    }
                default:
                    return null;
            }

        }
    }


}
