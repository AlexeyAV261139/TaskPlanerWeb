using TaskPlaner;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.UseStaticFiles();

app.UseMiddleware<RoutingMiddleware>();


app.Run();

