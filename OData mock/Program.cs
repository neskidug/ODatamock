using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using OData_mock.MiddleWare;
using OData_mock.Models;

var builder = WebApplication.CreateBuilder(args);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Odatamock>("Odatamocks");


builder.Services.AddControllers().AddOData(
    options => options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(
        routePrefix: "odata",
        model: modelBuilder.GetEdmModel()));
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxConcurrentConnections = 52;
    serverOptions.Limits.MaxConcurrentUpgradedConnections = 52;
});
var app = builder.Build();


app.UseODataRouteDebug();
app.UseRouting();
app.UseMiddleware<RequestLoggerMiddleware>();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();

