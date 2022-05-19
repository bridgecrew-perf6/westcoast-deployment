using Microsoft.EntityFrameworkCore;
using Vehicles_API.Data;
using Vehicles_API.Helpers;
using Vehicles_API.Interfaces;
using Vehicles_API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<VehicleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
// {
//     options.Password.RequireDigit = true;
//     options.Password.RequireLowercase = true;
//     options.Password.RequireUppercase = true;
//     options.Password.RequiredLength = 6;

//     options.Lockout.MaxFailedAccessAttempts = 5;
//     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
// }).AddEntityFrameworkStores<VehicleContext>();

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(
//             Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("apiKey"))
//         ),
//         ValidateLifetime = true,
//         ValidateAudience = false,
//         ValidateIssuer = false,
//         ClockSkew = TimeSpan.Zero
//     };
// });

// builder.Services.AddAuthorization(options =>
//     options.AddPolicy("Admins", policy => policy.RequireClaim("Administrator"))
// );

// Dependency injection configuration
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();

// Automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

var corsProfile = "WestCoastCors";

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsProfile,
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:5500");
        });
});

builder.Services.AddControllers();
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

app.UseCors(corsProfile);

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<VehicleContext>();
    await context.Database.MigrateAsync();
    await LoadData.LoadManufacturers(context);
    await LoadData.LoadVehicles(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Ett fel inträffade när migrering utfördes");
}

await app.RunAsync();
