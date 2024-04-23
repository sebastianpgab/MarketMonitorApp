namespace MarketMonitorApp.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string IdProduct { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ActualizationId { get; set; }
        public Actualization Actualization { get; set; }
    }
}
