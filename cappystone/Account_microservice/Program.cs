/*using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}*/


//using Account_microservice.DTO;
using account_microservice.Data;
using Account_microservice.Models;
using JwtAuthExtenstion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Account_microservice.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddJwtAuthentication();  // your extension

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddDbContext<AccountDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("CWMS_Accounts")
    ));

builder.Services.AddSwaggerGen();
/*
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AccountDbContext>()
    .AddDefaultTokenProviders();*/

// Identity
builder.Services
    .AddIdentityCore<ApplicationUser>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.Password.RequiredLength = 6;
        o.Password.RequireDigit = true;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
    })
   .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AccountDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager();

builder.Services.AddCors(o =>
{
    o.AddPolicy("ui", p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});


//builder.Services.AddScoped<IAccountRepository, AccountRepository>();


builder.Services.AddControllers();
//builder.Services.AddControllers().AddJsonOptions((options=>
//{
//  options.JsonSerializerOptions.ReferenceHandler
//}

//add authentication and authorization(can uncomment later)
//builder.Services.AddAuthentication();
//builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    await db.Database.MigrateAsync();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = new[] { "Worker", "Admin" };
    foreach (var role in roles)
    {
        if (!await roleMgr.RoleExistsAsync(role))
        {
            await roleMgr.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"Created role: {role}");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("ui");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
