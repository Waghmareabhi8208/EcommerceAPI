using Ecommerce.API.Data;
using Ecommerce.API.Interfaces;
using Ecommerce.API.Middleware;
using Ecommerce.API.Repositories;
using Ecommerce.API.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

// Logger Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval:
            RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = Microsoft.OpenApi.Models.ParameterLocation.Header,

            Description = "Enter JWT Token"
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});

// Add Database using dependency injection with the help of DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection"));
//});

// for deployment on railway conversion of database from SqlServer to PostgresSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Added dependency injection for product repository and ProductService
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();


// Register FluentValidation into Program.cs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Adding Token Validation Parameters using JwtBearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Register AuthService in Program.cs
builder.Services.AddScoped<IAuthService, AuthService>();

// Register CartService in Program.cs
builder.Services.AddScoped<ICartService, CartService>();

// Register OrderService in Program.cs
builder.Services.AddScoped<IOrderService, OrderService>();

// Register AddressService in Program.cs
builder.Services.AddScoped<IAddressService, AddressService>();

// Register PaymentService in Program.cs
builder.Services.AddScoped<IPaymentService,PaymentService>();

// Register Redis in Program.cs
//var redisConnectionString =
//    builder.Configuration["Redis:ConnectionString"];

//var redisOptions =
//    ConfigurationOptions.Parse(redisConnectionString!);

//redisOptions.AbortOnConnectFail = false;

//builder.Services.AddSingleton<IConnectionMultiplexer>(
//    ConnectionMultiplexer.Connect(redisOptions));

//var redisConnectionString =
//    builder.Configuration["Redis:ConnectionString"];

//if (!string.IsNullOrWhiteSpace(redisConnectionString))
//{
//    var redisOptions =
//        ConfigurationOptions.Parse(redisConnectionString);

//    redisOptions.AbortOnConnectFail = false;

//    builder.Services.AddSingleton<IConnectionMultiplexer>(
//        ConnectionMultiplexer.Connect(redisOptions));
//}

var redisHost = builder.Configuration["REDISHOST"];
var redisPort = builder.Configuration["REDISPORT"];
var redisPassword = builder.Configuration["REDISPASSWORD"];

// TEMP DEBUG LOGS
Console.WriteLine($"HOST={redisHost}");
Console.WriteLine($"PORT={redisPort}");
Console.WriteLine($"PASSWORD EXISTS={!string.IsNullOrEmpty(redisPassword)}");

var redisOptions = new ConfigurationOptions
{
    EndPoints = { $"{redisHost}:{redisPort}" },
    Password = redisPassword,
    AbortOnConnectFail = false
};

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisOptions));

// Register Rate Limiter Service
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (
        context,
        CancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsync(
            """
            {
                "message":
                "Too many requests. Please try again after 1 minute."
            }
            """,
           CancellationToken);
    };
    
    // Added independent policies for login,register and refresh token
    options.AddFixedWindowLimiter(
    "LoginPolicy",
    limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window =
            TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter(
        "RegisterPolicy",
        limiterOptions =>
        {
            limiterOptions.PermitLimit = 3;
            limiterOptions.Window =
                TimeSpan.FromMinutes(1);
            limiterOptions.QueueLimit = 0;
        });

    options.AddFixedWindowLimiter(
        "RefreshTokenPolicy",
        limiterOptions =>
        {
            limiterOptions.PermitLimit = 10;
            limiterOptions.Window =
                TimeSpan.FromMinutes(1);
            limiterOptions.QueueLimit = 0;
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

// Middleware added for global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// Middleware added to enable static files
app.UseStaticFiles();

// Middleware Added for authentication
app.UseAuthentication();

app.UseAuthorization();

// Middleware added for rate limiting
app.UseRateLimiter();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy",
        Time = DateTime.UtcNow
    });
});


app.Run();
