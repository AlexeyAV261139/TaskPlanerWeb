using TaskPlaner;
using TaskPlaner.Interfaces;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<IDbWorker, DbWorker>();

var app = builder.Build();


app.UseMiddleware<RoutingMiddleware>();


app.Run();

