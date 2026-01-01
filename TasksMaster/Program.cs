using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Services;
using Domain.Constants;
using Domain.Identities;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TasksMaster.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // ... (eventuele bestaande opties, zoals XML comments) ...

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Voer de JWT-token in de volgende indeling in: Bearer [de token]"
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
            new string[] {}
        }
    });
});

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

builder.Services.AddCors(options => {
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // De URL van je Angular app
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>(); // Zet de middleware helemaal bovenaan in de pipeline, zodat hij overal "omheen" zit

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Voer de Seeding Methode uit (die we hieronder definiëren)
    await SeedDataAsync(roleManager, userManager);
}

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");
// Enable authentication and authorization middlewares, make sure authentication comes before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

async Task SeedDataAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    // 1. Zorg dat de Rollen bestaan
    if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
    }
    if (!await roleManager.RoleExistsAsync(UserRoles.User))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
    }

    // 2. Maak een eerste Admin-gebruiker aan (optioneel, maar handig)
    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = "AdminUser",
            Email = "admin@mail.com",
            SecurityStamp = Guid.NewGuid().ToString()
        };

        await userManager.CreateAsync(newAdmin, "Azerty123!");

        // Ken de Admin rol toe
        await userManager.AddToRoleAsync(newAdmin, UserRoles.Admin);
    }
}

app.Run();
