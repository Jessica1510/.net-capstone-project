using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
//additional stuff to add 
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("workergateway.json", false, reloadOnChange: true).AddOcelot();

//add the ocelot config and other additional stuff
builder.Services.AddOcelot(builder.Configuration);
//add controllers since we dont choose it in the beginning 
builder.Services.AddControllers();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
//additional stuff to add 
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
await app.UseOcelot();

app.MapGet("/gateway", () =>
{
    return "welcome";
});

app.Run();

