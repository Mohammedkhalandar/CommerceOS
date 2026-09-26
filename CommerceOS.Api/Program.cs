using CommerceOS.Api.Middleware;
using CommerceOS.Application.BackgroundJobs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Application.Services;
using CommerceOS.Domain.Entities;
using CommerceOS.Infrastructure.Caching;
using CommerceOS.Infrastructure.Persistence;
using CommerceOS.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

// ======================================================
// SERILOG CONFIGURATION
// ======================================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/commerceos-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// SERILOG
// ======================================================

builder.Host.UseSerilog();

// ======================================================
// DATABASE
// ======================================================

builder.Services.AddDbContext<CommerceDbContext>(options =>
    options.UseMySQL(
        builder.Configuration.GetConnectionString("DefaultConnection")!
    )
);

// ======================================================
// REDIS CACHE
// ======================================================

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("Redis");
});

// ======================================================
// CONTROLLERS
// ======================================================

builder.Services.AddControllers();

// ======================================================
// SWAGGER
// ======================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CommerceOS.Api",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ======================================================
// JWT AUTHENTICATION
// ======================================================

var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException(
                 "JWT Key is not configured.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer is not configured.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
                  ?? throw new InvalidOperationException(
                      "JWT Audience is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
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
                    Encoding.UTF8.GetBytes(jwtKey)
                ),

            ClockSkew = TimeSpan.Zero
        };
});

// ======================================================
// REPOSITORIES
// ======================================================

builder.Services.AddScoped<ICustomerRepository, EfCustomerRepository>();

builder.Services.AddScoped<IProductRepository, EfProductRepository>();

builder.Services.AddScoped<
    IProductVariantRepository,
    EfProductVariantRepository>();

builder.Services.AddScoped<
    IInventoryRepository,
    EfInventoryRepository>();

builder.Services.AddScoped<
    IInventoryReservationRepository,
    EfInventoryReservationRepository>();

builder.Services.AddScoped<
    IOrderRepository,
    EfOrderRepository>();

builder.Services.AddScoped<
    IPaymentRepository,
    EfPaymentRepository>();

builder.Services.AddScoped<
    ICartRepository,
    EfCartRepository>();

// USER REPOSITORY

builder.Services.AddScoped<
    IUserRepository,
    EfUserRepository>();

// ======================================================
// TRANSACTION
// ======================================================

builder.Services.AddScoped<
    ICommerceTransaction,
    EfCommerceTransaction>();

// ======================================================
// CACHE
// ======================================================

builder.Services.AddScoped<
    ICacheService,
    RedisCacheService>();

// ======================================================
// APPLICATION SERVICES
// ======================================================

builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();

builder.Services.AddScoped<
    IProductService,
    ProductService>();

builder.Services.AddScoped<
    IProductVariantService,
    ProductVariantService>();

builder.Services.AddScoped<
    IInventoryService,
    InventoryService>();

builder.Services.AddScoped<
    IInventoryReservationService,
    InventoryReservationService>();

builder.Services.AddScoped<
    IOrderService,
    OrderService>();

builder.Services.AddScoped<
    IPaymentService,
    PaymentService>();

builder.Services.AddScoped<
    ICartService,
    CartService>();

// ======================================================
// AUTHENTICATION SERVICES
// ======================================================

builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

builder.Services.AddScoped<
    IAuthService,
    CommerceOS.Application.Services.AuthService>();

// ======================================================
// BACKGROUND JOBS
// ======================================================

builder.Services.AddSingleton<
    IBackgroundJobQueue,
    BackgroundJobQueue>();

builder.Services.AddHostedService<
    OrderBackgroundWorker>();

// ======================================================
// BUILD APPLICATION
// ======================================================

var app = builder.Build();

// ======================================================
// SERILOG REQUEST LOGGING
// ======================================================

app.UseSerilogRequestLogging();

// ======================================================
// HTTP PIPELINE
// ======================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ======================================================
// GLOBAL EXCEPTION HANDLING
// ======================================================

app.UseMiddleware<GlobalExceptionMiddleware>();

// ======================================================
// AUTHENTICATION & AUTHORIZATION
// ======================================================

app.UseAuthentication();
app.UseAuthorization();

// ======================================================
// CONTROLLERS
// ======================================================

app.MapControllers();

// ======================================================
// RUN
// ======================================================

app.Run();