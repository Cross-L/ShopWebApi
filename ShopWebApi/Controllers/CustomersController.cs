using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopWebApi.Data;
using ShopWebApi.DTOs;

namespace ShopWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController(ShopDbContext context) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of customers whose birthday matches the specified date.
    /// </summary>
    /// <param name="date">The date to check for birthdays (format: YYYY-MM-DD).</param>
    /// <returns>A list of customers (Id, FullName).</returns>
    [HttpGet("birthdays")]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult GetBirthdayCustomers([FromQuery] DateTime date)
    {
        if (date == default)
        {
            return BadRequest("Date cannot be empty.");
        }

        var customers = context.Customers
            .FromSqlRaw(
                """
                SELECT "Id", "FullName", "BirthDate", "RegistrationDate"
                              FROM "Customers"
                              WHERE EXTRACT(DAY FROM "BirthDate") = {0} AND EXTRACT(MONTH FROM "BirthDate") = {1}
                """,
                date.Day, date.Month)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName
            })
            .ToList();

        return Ok(customers);
    }

    /// <summary>
    /// Retrieves a list of customers who made purchases in the last N days.
    /// </summary>
    /// <param name="days">Number of days (must be greater than 0).</param>
    /// <returns>A list of customers (Id, FullName, LastPurchase).</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<RecentCustomerDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult GetRecentCustomers([FromQuery][Range(1, int.MaxValue)] int days)
    {
        if (days <= 0)
        {
            return BadRequest("Number of days must be greater than 0.");
        }

        var dateThreshold = DateTime.UtcNow.AddDays(-days);

        var customers = context.Orders
            .Where(o => o.OrderDate >= dateThreshold)
            .GroupBy(o => o.Customer)
            .Select(g => new RecentCustomerDto
            {
                Id = g.Key.Id,
                FullName = g.Key.FullName,
                LastPurchase = g.Max(o => o.OrderDate)
            })
            .ToList();

        return Ok(customers);
    }

    /// <summary>
    /// Retrieves a list of product categories purchased by the specified customer, along with the total quantity.
    /// </summary>
    /// <param name="customerId">The identifier of the customer.</param>
    /// <returns>A list of categories with the total quantity purchased.</returns>
    [HttpGet("{customerId:int}/categories")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDemandDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult GetDemandedCategories(int customerId)
    {
        if (customerId <= 0)
        {
            return BadRequest("Customer ID must be greater than 0.");
        }

        var customerExists = context.Customers.Any(c => c.Id == customerId);
        if (!customerExists)
        {
            return NotFound($"Customer with Id {customerId} not found.");
        }

        var categories = context.OrderItems
            .Where(oi => oi.Order.CustomerId == customerId)
            .GroupBy(oi => oi.Product.Category)
            .Select(g => new CategoryDemandDto
            {
                Category = g.Key,
                TotalQuantity = g.Sum(oi => oi.Quantity)
            })
            .ToList();

        return Ok(categories);
    }
}