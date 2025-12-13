using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Services;
using Domain.Identities;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Configuratie van Database Context
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlite("Data Source=TasksMaster.db"));

// Configuratie van .NET Identity (ZEER BELANGRIJK: Hier wordt UserManager geregistreerd!)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskContext>()
    .AddDefaultTokenProviders();

// Haal de Secret Key op
var secret = builder.Configuration["JwtSettings:Secret"];
var keyBytes = Encoding.UTF8.GetBytes(secret);
var signingKey = new SymmetricSecurityKey(keyBytes);

// Configuratie van JWT Bearer Authenticatie
builder.Services.AddAuthentication(options =>
{
    // Stel het standaard schema in voor authenticatie
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 1. Sleutel validatie
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,

        // 2. Issuer/Audience validatie (optioneel, maar goed voor beveiliging)
        // Je kunt deze op false zetten als je ze niet in appsettings hebt geconfigureerd.
        ValidateIssuer = false, // We valideren de uitgever niet
        ValidateAudience = false, // We valideren de ontvanger niet

        // 3. Lifetime validatie
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Zorg dat de vervaltijd nauwkeurig is
    };
});

// Middlewares, Registratie van Custom Services
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable authentication and authorization middlewares, make sure authentication comes before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
