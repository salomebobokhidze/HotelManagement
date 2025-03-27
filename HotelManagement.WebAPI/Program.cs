using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using HotelManagement.Core.Entities;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Application.Services;
using HotelManagement.Core.Interfaces;
using HotelManagement.Core.Services;
using HotelManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Guest, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure Authentication with JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Add your services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IRoomService, RoomService>();


builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<IGenericRepository<Manager>, GenericRepository<Manager>>();
builder.Services.AddScoped<IGenericRepository<Guest>, GenericRepository<Guest>>();
builder.Services.AddScoped<IGenericRepository<Hotel>, GenericRepository<Hotel>>();
builder.Services.AddScoped<IGenericRepository<Room>, GenericRepository<Room>>();
builder.Services.AddScoped<IGenericRepository<Reservation>, GenericRepository<Reservation>>();




// Add controllers
builder.Services.AddControllers();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Management API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedRolesAndAdmin(serviceProvider);
}

app.Run();

async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Guest>>();

    // Seed roles
    string[] roleNames = { "Admin", "Guest" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Seed admin user
    var adminEmail = "admin@hotel.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new Guest
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            PersonalNumber = "00000000000",
            PhoneNumber = "+1234567890",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@1234");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}