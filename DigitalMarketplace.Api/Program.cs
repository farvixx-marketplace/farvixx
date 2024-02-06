using DigitalMarketplace.Api.Middleware;
using DigitalMarketplace.Core.Models;
using DigitalMarketplace.Infrastructure;
using DigitalMarketplace.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

bool devMode = builder.Environment.IsDevelopment();
string connectionString = builder.Configuration
    .GetConnectionString($"{(devMode ? "Development" : "Production")}PostgreSql")!;

string mailjetKey = builder.Configuration.GetSection("MailjetConfig:ApiKey").Value!;
string mailjetSecret = builder.Configuration.GetSection("MailjetConfig:ApiSecret").Value!;

builder.Services.AddInfrastructure(connectionString, mailjetKey, mailjetSecret);

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtConfig:Key").Value!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtConfig:Key")!)),
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidIssuers = builder.Configuration.GetSection("JwtConfig:Issuer").Value?.Split(';'),
            ValidAudiences = builder.Configuration.GetSection("JwtConfig:Audience").Value?.Split(';')
        };
    });

builder.Services.AddAuthorization();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<AccessTokenMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
