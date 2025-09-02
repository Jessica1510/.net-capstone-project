using Microsoft.EntityFrameworkCore;
using Worker_microservice.Data;
using Worker_microservice.Repository;
using JwtAuthExtenstion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
//  Add DB Context to the Application
builder.Services.AddDbContext<WorkerDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("CWMS_Worker")
    ));
// ADD repositry Service Instance 
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();

//  Add the Mapper Services
builder.Services.AddAutoMapper(typeof(Program));
// add the memory Cache service in application
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

builder.Services.AddJwtAuthentication();
builder.Services.AddAuthorization();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ui");
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
