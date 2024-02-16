using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
string connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new Exception("�� ������� ������ �����������");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapGet("/api/tasks", async (ApplicationContext db) => await db.MyTasks.ToListAsync());

app.MapGet("/api/tasks/{id:guid}", async (string id, ApplicationContext db) =>
{
    MyTask? task = await db.MyTasks.FirstOrDefaultAsync(t => t.Id == id);

    if (task == null) Results.NotFound(new { messege = "������ �� �������" });

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

    if (task == null) return Results.NotFound(new { message = "������������ �� ������" });

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
    if (task == null) return Results.NotFound(new { message = "������������ �� ������" });

    db.MyTasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.Json(task);
});

app.Run();

