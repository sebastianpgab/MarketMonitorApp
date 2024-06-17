using MarketMonitorApp;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services;
using MarketMonitorApp.Services.ProductPatterns;
using MarketMonitorApp.Services.ProductsStrategy;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MarketMonitorDbContext>();
builder.Services.AddScoped<SeedData>();
builder.Services.AddScoped<IDistributorDetailsService, DistributorDetailsService>();
builder.Services.AddScoped<IDistributorStrategy, TwojaBronStrategy>();
builder.Services.AddScoped<IDistributorStrategySelector, DistributorStrategySelector>();

builder.Services.AddScoped<IPriceScraper, PriceScraper>();



var app = builder.Build();
var scope = app.Services.CreateScope();
var seedData = scope.ServiceProvider.GetRequiredService<SeedData>();


// Configure the HTTP request pipeline.
seedData.Initialize();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
