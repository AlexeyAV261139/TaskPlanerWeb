using TaskPlaner;
using TaskPlaner.Interfaces;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IDbWorker, DbWorker>();

var app = builder.Build();

app.UseStaticFiles();

app.UseMiddleware<RoutingMiddleware>();


app.Run();

