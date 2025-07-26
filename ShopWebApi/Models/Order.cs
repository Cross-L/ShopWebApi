namespace ShopWebApi.Models;

public class Order
{
    public int Id { get; set; }
    public required string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; } 
    public decimal TotalCost { get; set; } 
    public int CustomerId { get; set; } 
    public Customer Customer { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = [];
}