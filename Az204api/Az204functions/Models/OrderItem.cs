namespace Az204functions.Models
{
    public class OrderItem
    {
        public string Id { get; set; }
        public string CoffeeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
