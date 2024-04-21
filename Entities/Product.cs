namespace MarketMonitorApp.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string IdProduct { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }
    }
}
