namespace PortfolioERP.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? TaxCode { get; set; }

    public string? VatNumber { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Province { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = "Italy";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<SalesOrder> Orders { get; set; }
    = new List<SalesOrder>();
}