﻿using MarketMonitorApp.Entities;
using MarketMonitorApp.Services.ProductsStrategy;

namespace MarketMonitorApp.Services.ProductPatterns
{
    public interface IDistributorStrategySelector
    {
        public IDistributorStrategy ChoseStrategy(Distributor distributor);
    }
    public class DistributorStrategySelector : IDistributorStrategySelector
    {
        public IDistributorStrategy ChoseStrategy(Distributor distributor)
        {
            switch (distributor.Name)
            {
                case ("Twoja Bron"):
                {
                    return new TwojaBronStrategy();
                }
                case ("Hubertus Białystok"):
                {
                    return new HubertusBialystokStrategy();
                }
                case ("Delta Optical"):
                {
                    return new DeltaOpticalStrategy();
                }
                case ("Szuster"):
                {
                    return new SzusterStrategy();
                }
                case ("Knieja"):
                {
                    return new KniejaStrategy();
                }
                default:
                    return null;
            }

        }
    }


}
