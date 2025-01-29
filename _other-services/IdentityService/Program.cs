using Common.ServiceDefaults;
using IdentityService.Api.Configuration;
using IdentityService.Api.Services;
using IdentityService.Domain.AppRoleAggregate;
using IdentityService.Domain.AppUserAggregate;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();

// Add services to the container.
RegisterServices(builder.Services, builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Seed Identity Roles
await SeedIdentityAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("")
    .MapIdentityApi<IdentityUser>();

app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
    // Add Authentication and Authorization services
    services.AddAuthorization();
    services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://localhost:5011",
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                
            };
        });

    services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<IdentityServiceContext>()
        .AddDefaultTokenProviders()
        .AddDefaultUI();
    
    services.AddDbContext<IdentityServiceContext>(options =>
    {
        options.UseSqlServerDbContext(
            configuration.GetConnectionString("IdentityContextSqlServerConnectionString")!);
    });

    services.AddScoped<TokenService>();
}

static async Task SeedIdentityAsync(IServiceProvider serviceProvider)
{
    string[] roles = ["Admin", "User"];
    
    using var scope = serviceProvider.CreateScope();
    using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    
    foreach (var role in roles)
    {
        bool roleExists = await roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new AppRole(role));
        }
    }

    var adminUser = await userManager.FindByEmailAsync("admin@jschwartz.live");
    if (adminUser is null)
    {
        var user = new AppUser
        {
            UserName = "admin",
            Email = "admin@jschwartz.live",
            EmailConfirmed = true
        };
        
        await userManager.CreateAsync(user, "Password123!");
        adminUser = await userManager.FindByEmailAsync("admin@jschwartz.live");
    }

    var isAdmin = await userManager.IsInRoleAsync(adminUser!, "Admin");
    if (!isAdmin)
    {
        await userManager.AddToRoleAsync(adminUser!, "Admin");
    }
    
    var isUser = await userManager.IsInRoleAsync(adminUser!, "User");
    if (!isUser)
    {
        await userManager.AddToRoleAsync(adminUser!, "User");
    }
}