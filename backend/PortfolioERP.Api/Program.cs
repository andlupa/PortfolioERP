using Microsoft.EntityFrameworkCore;

using System.Text;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using PortfolioERP.Infrastructure.Persistence;
using PortfolioERP.Infrastructure.Services;
using PortfolioERP.Api.Filters;
using PortfolioERP.Api.Middlewares;
using PortfolioERP.Domain.Services.Orders;
using PortfolioERP.Domain.Security;
using PortfolioERP.Application.Features.Authentication;
using PortfolioERP.Application.Features.Categories;
using PortfolioERP.Application.Features.Products;
using PortfolioERP.Application.Features.Customers;
using PortfolioERP.Application.Features.Orders;
using PortfolioERP.Application.Features.PurchaseOrders;
using PortfolioERP.Application.Features.Suppliers;
using PortfolioERP.Application.Features.Inventory;
using PortfolioERP.Application.Features.Dashboard;
using PortfolioERP.Application.Common;
using PortfolioERP.Application.Common.Messaging;

using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Legge la configurazione per i logs
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

// Recupera la connection string (Nel nostro caso è impostata negli user-secrets di dotnet
// e caricata in una variabile di ambiente
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

// Registra AppDbContext.
// Quando verrà creato per la prima volta avrà queste impostazioni
// sqlOptions.EnableRetryOnFailure(); Riprova se errori db temporanei. Utile con AzureSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString, sqlOptions => { sqlOptions.EnableRetryOnFailure(); }));

// Per PostgreSQL
// builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Registra i validator nella DI.
// Cerca tutte le classi AbstractValidator<T> nell'assembly
// che contiene CreateCategoryRequestValidator
// non è necessario registrarle a mano una per una
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>();

//Permette ai servizi di accedere all'HttpContext corrente.
builder.Services.AddHttpContextAccessor();

// Configura il client http del microservizio Inventory
builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(
    client =>
    {
        // Recupera l'url dall'appsetting o variabili di ambiente
        var inventoryServiceUrl =
            builder.Configuration["Services:InventoryService"]
            ?? throw new InvalidOperationException(
                "InventoryService URL is not configured.");

        // imposta l'url nel client http, per poter inoltrare le richieste
        client.BaseAddress = new Uri(inventoryServiceUrl);
    });

// Registra nella DI i vari servizi
// AddScoped: per ogni richiesta http viene creata una nuova istanza
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUserService, UserService>();
// registra il fluent validator
builder.Services.AddScoped<FluentValidationFilter>();
// Per IEventPublisher registra RabbitMq
builder.Services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();
// Oggetto per inserire dati iniziali nel database in sviluppo
builder.Services.AddScoped<DevelopmentDataSeeder>();
// AddSingleton: unica istanza per tutta l'applicazione
// é stateless, non conserva dati specifici
builder.Services.AddSingleton<IOrderCalculator, OrderCalculator>();

// abilita il sistema MVC/API Controllers
// registra tutte le classi [ApiController]
builder.Services.AddControllers(options =>
{
    // registra il fluent validator su tutti i controllers
    options.Filters.Add<FluentValidationFilter>();
});

// Queste istruzioni permettono ad ASP.NET di descrivere gli endpoint dell'applicazione.
// Swagger utilizzerà queste informazioni per costruire la documentazione.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Registrazione di Swagger Generator.
builder.Services.AddSwaggerGen(options =>
{
    // aggiunge l'autenticator bearer
    // e configura lo schema che dovrà usare Swagger
    // Swagger mostrerà il pulsante authorize
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Inserisci il JWT ottenuto dal login."
        });

    // Dice a Swagger che il bearer sarà
    // applicato agli endpoint API
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document),
                []
            }
        });
});

// Registra il Cross-Origin Resource Sharing
// per le chiamate dal frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            // Indirizzi autorizzati ad inviare le richieste
            .WithOrigins(
                "http://localhost:4200", //Angular locale
                "https://portfolioerp.pages.dev" //frontend Cloudflare
            )
            // Permesso per qualsiasi header e metodo HTTP
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ** LOGIN **
// Redistra i servizi di autenticazione
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Legge dalle variabili di ambiente (Secret users) 
// la chiave JWT
var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is not configured.");
// l'issuer - chi lo emette
var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuer is not configured.");
// l'audience - a chi è destinato
var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audience is not configured.");

// configura l'autenticazione HTTP
builder.Services
    // metodo predefinito: lo schema JwtBearer
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // configura come validare il bearer
    .AddJwtBearer(options =>
    {
        // registra gli eventi OnAuthenticationFailed e OnTokenValidated
        options.Events = new JwtBearerEvents
        {
            // se fallisce l'autenticazione scrive un messaggio nella console
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT authentication failed: {context.Exception}");

                return Task.CompletedTask;
            },
            // se l'autenticazione funziona scrive un messaggio nella console
            OnTokenValidated = context =>
            {
                Console.WriteLine($"JWT validated for: {context.Principal?.Identity?.Name}");

                return Task.CompletedTask;
            }
        };
        // configura come validare il bearer
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // cosa validare
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                // imposta issuer e audience corretti
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                // jwtKey viene trasformata in byte e usata
                // per verificare la firma crittografica
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                // tolleranza scadenza token
                // Zero = Quando il token scade, deve essere considerato scaduto immediatamente
                ClockSkew = TimeSpan.Zero
            };
    });

// Configura le autorizzazioni
builder.Services.AddAuthorization(options =>
{
    // Crea policy per [Authorize(Policy = "CanWrite")]
    // solo per Admin e User. Demo non potrà usare quegli endpoints
    options.AddPolicy("CanWrite", policy =>
    {
        policy.RequireRole(
            AppRoles.Admin,
            AppRoles.User);
    });
});


// ASP.NET costruisce l'applicazione usando tutte le configurazioni effettuate
var app = builder.Build();

// log di avvio
app.Logger.LogInformation("PortfolioERP API started in {Environment}", 
    app.Environment.EnvironmentName);

// Ogni chiamata http produce un log tipo
//    HTTP GET /api/products responded 200 in 34 ms
app.UseSerilogRequestLogging();

// Qualsiasi eccezione dell'applicazione verrà gestita
// nella classe ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Usa Swagger solo in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Commentalo per ora, visto il warning HTTPS
// Serve per dirigere le chiamate http a https
// app.UseHttpsRedirection();

// Applica la policy per l'origine delle chiamate HTTP
app.UseCors("Frontend");

// Per ogni richiesta valida il bearer
// se valida costruisce HttpContext.User con tutte le informazioni
app.UseAuthentication();
// Dice all'app di controllare i permessi degli endpoint HTTP
// [Authorize], [Authorize(Roles = "Admin")], [Authorize(Policy = "CanWrite")]
app.UseAuthorization();

// ASP.NET collega le route ai Controllers,
// vengono effettivamente resi disponibili gli endpoint.
app.MapControllers();

// Aggiunta dati di configurazione in fase di sviluppo
if (app.Environment.IsDevelopment())
{
    const int maxAttempts = 3;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            // crea lo scope manualmente (HTTP requests lo creano automaticamente)
            using var scope = app.Services.CreateScope();
            // Fa l'injection della classe DevelopmentDataSeeder
            var seeder =
                scope.ServiceProvider
                    .GetRequiredService<DevelopmentDataSeeder>();
            // Chiama il metodo per creare l'utente Admin
            // per avere automaticamente un utente con cui fare login durante lo sviluppo
            await seeder.SeedAsync();

            app.Logger.LogInformation(
                "Development data seeding completed.");

            break;
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(
                ex,
                "Development data seeding attempt {Attempt}/{MaxAttempts} failed.",
                attempt,
                maxAttempts);

            if (attempt == maxAttempts)
            {
                app.Logger.LogWarning(
                    "Development data seeding abandoned. " +
                    "The API will continue running.");

                break;
            }

            await Task.Delay(
                TimeSpan.FromSeconds(10));
        }
    }
}


// avvia il web server e aspetta richieste HTTP
app.Run();

//Il percorso all'arrivo di una richiesta HTTP:

//Angular
//  │
//  │ HTTP POST
//  ▼
//PortfolioERP API
//  │
//  ▼
//Serilog Request Logging
//  │
//  ▼
//ExceptionHandlingMiddleware
//  │
//  ▼
//CORS
//  │
//  ▼
//Authentication
//  │
//  │ legge JWT
//  ▼
//HttpContext.User
//  │
//  ▼
//Authorization
//  │
//  ▼
//Routing / Controller
//  │
//  ▼
//FluentValidationFilter
//  │
//  ▼
//CreateCategoryRequestValidator
//  │
//  │ valido
//  ▼
//CategoriesController
//  │
//  ▼
//ICategoryService
//  │
//  ▼
//CategoryService
//  │
//  ▼
//AppDbContext
//  │
//  ▼
//EF Core
//  │
//  ▼
//Azure SQL