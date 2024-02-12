using TaskPlaner;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();


app.UseMiddleware<RoutingMiddleware>();


app.Run();

