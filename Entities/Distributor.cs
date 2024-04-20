namespace MarketMonitorApp.Entities
{
    public class Distributor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
