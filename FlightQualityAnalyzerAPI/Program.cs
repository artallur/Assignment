using FlightQualityAnalyzerAPI.Helpers;
using FlightQualityAnalyzerAPI.Interfaces;
using FlightQualityAnalyzerAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
string flightsCSVFilePath = @"Data\flights.csv";
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IFlightService, FlightService>(provider =>
{
    var memoryCache = provider.GetRequiredService<IMemoryCache>();
    return new FlightService(memoryCache, flightsCSVFilePath);
});

builder.Services.AddScoped<IFlightAnalyser, FlightAnalyzer>(provider =>
{
    var memoryCache = provider.GetRequiredService<IMemoryCache>();
    return new FlightAnalyzer(memoryCache, flightsCSVFilePath);
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
