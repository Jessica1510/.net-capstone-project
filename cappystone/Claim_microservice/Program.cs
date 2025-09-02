using Microsoft.EntityFrameworkCore;
using Claim_microservice.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ✅ Add Swagger generation service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ClaimDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(o =>
{
    o.AddPolicy("ui", p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // ✅ Enable Swagger middleware
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("ui");
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
