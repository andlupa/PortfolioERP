using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Microsoft.EntityFrameworkCore;
using PortfolioERP.InvoiceService.Persistence;
using PortfolioERP.InvoiceService.Messaging;
using PortfolioERP.InvoiceService.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString =
    builder.Configuration
        .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not configured.");

// Aggiunge DbContext ai servizi con la stringa di connessione
// e le opzioni di retry in caso di fallimento.
builder.Services.AddDbContext<InvoiceDbContext>(
    options =>
        options.UseSqlServer(
            connectionString,
            sqlOptions =>
                sqlOptions.EnableRetryOnFailure()));

builder.Services.AddHostedService<OrderShippedConsumer>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://portfolioerpinv.pages.dev"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is not configured.");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuer is not configured.");

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audience is not configured.");

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseCors("ReactFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.UseHttpsRedirection();

app.Run();


