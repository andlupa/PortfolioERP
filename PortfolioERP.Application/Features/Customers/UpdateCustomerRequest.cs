using System.ComponentModel.DataAnnotations;

namespace PortfolioERP.Application.Features.Customers;

public sealed record UpdateCustomerRequest(
    [Required]
    [MaxLength(30)]
    string CustomerCode,

    [Required]
    [MaxLength(200)]
    string CompanyName,

    [MaxLength(100)]
    string? FirstName,

    [MaxLength(100)]
    string? LastName,

    [MaxLength(16)]
    string? TaxCode,

    [MaxLength(20)]
    string? VatNumber,

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    string Email,

    [MaxLength(30)]
    string? Phone,

    [MaxLength(250)]
    string? Address,

    [MaxLength(100)]
    string? City,

    [MaxLength(10)]
    string? Province,

    [MaxLength(15)]
    string? PostalCode,

    [Required]
    [MaxLength(100)]
    string Country,

    bool IsActive);