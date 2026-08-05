using Microsoft.EntityFrameworkCore;
using PortfolioERP.Infrastructure.Persistence;
using PortfolioERP.Application.Features.Categories;
using PortfolioERP.Application.Features.Products;
using PortfolioERP.Application.Features.Customers;
using PortfolioERP.Infrastructure.Services;
using PortfolioERP.Api.Middlewares;
using PortfolioERP.Domain.Services.Orders;
using PortfolioERP.Application.Features.Orders;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IOrderCalculator, OrderCalculator>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IOrderCalculator, OrderCalculator>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();