using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskPlaner.DB;
using TaskPlaner.DB.Entityes;

public static class RoutingManager
{
    public static void Auth(WebApplication app)
    {
        app.MapGet("/login", async (context) =>
        {

            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync("wwwroot/html/Authorization.html");
        });

        app.MapPost("/login", (Person loginData, ApplicationContext db) =>
        {
            Person? person = db.Persons.FirstOrDefault(p => p.Email == loginData.Email &&
            p.Password == loginData.Password);

            if (person == null) return Results.Unauthorized();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };

            var jwt = new JwtSecurityToken(
                   issuer: AuthOptions.ISSUER,
                   audience: AuthOptions.AUDIENCE,
                   claims: claims,
                   expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                   signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = person.Email
            };

            return Results.Json(response);
        });


        app.Map("/data", [Authorize] () => new { message = "Hello World!" });
    }

    public static void StartRouting(WebApplication app)
    {   

        app.MapGet("/api/tasks", async (ApplicationContext db) => await db.MyTasks.ToListAsync());

        app.MapGet("/api/tasks/{id:guid}", async (Guid id, ApplicationContext db) =>
        {
            MyTask? task = await db.MyTasks.FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) Results.NotFound(new { messege = "Задача не найдена" });

            return Results.Json(task);
        });

        app.MapPost("/api/tasks", async (MyTask task, ApplicationContext db) =>
        {
            await db.MyTasks.AddAsync(task);
            await db.SaveChangesAsync();
            return task;
        });

        app.MapPut("/api/tasks", async (MyTask taskData, ApplicationContext db) =>
        {
            var task = await db.MyTasks.FirstOrDefaultAsync(t => t.Id == taskData.Id);

            if (task == null) return Results.NotFound(new { message = "Пользователь не найден" });

            task.Heading = taskData.Heading;
            task.Content = taskData.Content;
            task.Date = taskData.Date;
            task.Priority = taskData.Priority;
            db.SaveChanges();
            return Results.Json(task);
        });

        app.MapDelete("/api/tasks/{id:guid}", async (Guid id, ApplicationContext db) =>
        {
            MyTask? task = await db.MyTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return Results.NotFound(new { message = "Пользователь не найден" });

            db.MyTasks.Remove(task);
            await db.SaveChangesAsync();
            return Results.Json(task);
        });
    }

}