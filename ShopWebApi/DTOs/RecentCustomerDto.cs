namespace ShopWebApi.DTOs;

public class RecentCustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime LastPurchase { get; set; }
}