namespace MarketMonitorApp.Entities
{
    public class Actualization
    {
        public int Id { get; set; }
        public bool IsEntered { get; set; }
        public DateTime LastActualization { get; set; } = DateTime.Now;
        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
