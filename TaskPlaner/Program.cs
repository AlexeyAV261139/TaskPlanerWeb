using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var people = new List<Person>
 {
    new Person("tom@gmail.com", "12345"),
    new Person("bob@gmail.com", "55555")
};

var builder = WebApplication.CreateBuilder();
string connection = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new Exception("Ќе указана строка подключени€");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироватьс€ издатель при валидации токена
            ValidateIssuer = true,
            // строка, представл€юща€ издател€
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироватьс€ потребитель токена
            ValidateAudience = true,
            // установка потребител€ токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироватьс€ врем€ существовани€
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидаци€ ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/login", async (context) =>
{

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/html/Authorization.html");
});

app.MapPost("/login", (Person loginData) =>
{
    Person? person = people.FirstOrDefault(p => p.Email == loginData.Email &&
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

#region
app.MapGet("/api/tasks", async (ApplicationContext db) => await db.MyTasks.ToListAsync());

app.MapGet("/api/tasks/{id:guid}", async (string id, ApplicationContext db) =>
{
    MyTask? task = await db.MyTasks.FirstOrDefaultAsync(t => t.Id == id);

    if (task == null) Results.NotFound(new { messege = "«адача не найдена" });

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

    if (task == null) return Results.NotFound(new { message = "ѕользователь не найден" });

    task.Heading = taskData.Heading;
    task.Content = taskData.Content;
    task.Date = taskData.Date;
    task.Priority = taskData.Priority;
    db.SaveChanges();
    return Results.Json(task);
});

app.MapDelete("/api/tasks/{id:guid}", async (string id, ApplicationContext db) =>
{
    MyTask? task = await db.MyTasks.FirstOrDefaultAsync(t => t.Id == id);
    if (task == null) return Results.NotFound(new { message = "ѕользователь не найден" });

    db.MyTasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.Json(task);
});
#endregion 
app.Run();

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ дл€ шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}

record class Person(string Email, string Password);