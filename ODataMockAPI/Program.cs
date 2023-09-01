using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using OData_mock.Controllers;
using OData_mock.Interfaces;
using OData_mock.MiddleWare;
using OData_mock.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Odatamock>("Odatamocks");


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OdatamocksController>();

builder.Services.AddControllers().AddOData(
    options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
        routePrefix: "odata",
        model: modelBuilder.GetEdmModel()));

//Configure Serilog logger
var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .WriteTo.File("C:\\mylogs\\log.txt", rollingInterval: RollingInterval.Minute)
    .CreateLogger();

builder.Services.AddLogging(LoggingBuilder =>
{
    LoggingBuilder.ClearProviders();
    LoggingBuilder.AddSerilog();
});

//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.Limits.MaxConcurrentConnections = 5;
//    serverOptions.Limits.MaxConcurrentUpgradedConnections = 5;
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseODataRouteDebug();
app.UseRouting();
//app.UseMiddleware<RequestLoggerMiddleware>();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();

