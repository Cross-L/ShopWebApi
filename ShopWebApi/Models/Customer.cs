namespace ShopWebApi.Models;

public class Customer
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime RegistrationDate { get; set; }
    public List<Order> Orders { get; set; } = [];
}