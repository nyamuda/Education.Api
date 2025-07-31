using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DrivingSchool.API.Services.Implementations.Auth;
using Education.Api.Data;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Auth;
using Education.Api.Services.Abstractions.Email;
using Education.Api.Services.Abstractions.Users;
using Education.Api.Services.Implementations.Auth;
using Education.Api.Services.Implementations.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddSingleton<IEmailTemplateBuilder, EmailTemplateBuilder>();


//Configure the database
string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Missing configuration: 'DefaultConnection' is not set."
    );
builder
    .Services
    .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Binds the "Company" section from appsettings.json to the Company class
// This allows you to access company config settings in a strongly-typed way
builder.Services.Configure<Company>(builder.Configuration.GetSection("Company"));

// Binds the "SmtpSettings" section from appsettings.json to the SmtpSettings class
// This allows you to access SMTP config settings in a strongly-typed way
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

//Configure JWT Authentication
string jwtIssuer =
    builder.Configuration.GetValue<string>("Authentication:JwtSettings:Issuer")
    ?? throw new InvalidOperationException(
        "Missing configuration: 'Authentication:JwtSettings:Issuer' is not set."
    );

string jwtAudience =
    builder.Configuration.GetValue<string>("Authentication:JwtSettings:Audience")
    ?? throw new InvalidOperationException(
        "Missing configuration: 'Authentication:JwtSettings:Audience' is not set."
    );

string jwtKey =
    builder.Configuration.GetValue<string>("Authentication:JwtSettings:Key")
    ?? throw new InvalidOperationException(
        "Missing configuration: 'Authentication:JwtSettings:Key' is not set."
    );

builder
    .Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

//Add CORS policy
var AllowAnyOrigin = "_allowAnyOrigin";
builder
    .Services
    .AddCors(options =>
    {
        options.AddPolicy(
            name: AllowAnyOrigin,
            policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
        );
    });

//JSON Serialization options
builder
    .Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // serialize enums as strings in api responses
        options
            .JsonSerializerOptions
            .Converters
            .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        //handle Cycles
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure OpenApi
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// CORS middleware (should come before any auth)
app.UseCors(AllowAnyOrigin);

// Authentication middleware
app.UseAuthentication();

// Authorization middleware
app.UseAuthorization();

app.MapControllers();

//root API route and its handler
app.MapGet("/api", () => "Welcome to the Education API.");

app.Run();
