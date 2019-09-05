namespace SalesAnalytics.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}
