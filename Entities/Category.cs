namespace MarketMonitorApp.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LinkToCategory { get; set; }
        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }
    }
}
