using Microsoft.EntityFrameworkCore;
using Course_microservice.Data;
using JwtAuthExtenstion;

var builder = WebApplication.CreateBuilder(args);

var accountServiceUrl = builder.Configuration["Services:AccountMicroservice"];

builder.Services.AddHttpClient<Course_microservice.Services.UserService>(client =>
{
    client.BaseAddress = new Uri(accountServiceUrl);
});

builder.Services.AddJwtAuthentication();

// Add services to the container.
builder.Services.AddControllers();

// ✅ Add Swagger generation service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",   // Angular dev server (default)
                "http://localhost:5173",   // Vite/other local frontends (optional)
                "https://localhost:4200"   // If you run Angular over HTTPS locally
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        // NOTE: If you need cookies/Authorization headers cross-site, add:
        // .AllowCredentials();
        // and DO NOT use AllowAnyOrigin() together with AllowCredentials()
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

// ✅ Use the CORS policy BEFORE MapControllers()
app.UseCors("AllowFrontend");

// (If you use auth later)
 app.UseAuthentication();
 app.UseAuthorization();

app.MapControllers();

app.Run();
