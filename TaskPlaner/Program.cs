using Microsoft.Extensions.FileProviders;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.UseStaticFiles();

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;
    //string expressionForNumber = "^/api/users/([0-9]+)$";   // если id представл€ет число

    // 2e752824-1657-4c7f-844b-6ec2e168e99c
    string expressionForGuid = @"^/api/tasks/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/tasks" && request.Method == "GET")
    {
        await GetAllMyTasks(response);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
    {
        // получаем id из адреса url
        string? id = path.Value?.Split("/")[3];
        await GetMyTask(id, response);
    }
    else if (path == "/api/tasks" && request.Method == "POST")
    {
        await CreateMyTask(response, request);
    }
    else if (path == "/api/tasks" && request.Method == "PUT")
    {
        await UpdateMyTask(response, request);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
    {
        string? id = path.Value?.Split("/")[3];
        await DeleteMyTask(id, response);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("wwwroot/html/index.html");
    }
});

app.Run();

// получение всех пользователей
async Task GetAllMyTasks(HttpResponse response)
{
    using ApplicationContext db = new ApplicationContext();
    {
        await response.WriteAsJsonAsync(db.MyTasks);
    }
}
// получение одного пользовател€ по id
async Task GetMyTask(string? id, HttpResponse response)
{
    using (ApplicationContext db = new ApplicationContext())
    {
        // получаем пользовател€ по id
        MyTask? myTask = db.MyTasks.FirstOrDefault((t) => t.Id == id);
        // если пользователь найден, отправл€ем его
        if (myTask != null)
            await response.WriteAsJsonAsync(myTask);
        // если не найден, отправл€ем статусный код и сообщение об ошибке
        else
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(new { message = "«адача не найдена" });
        }
    }
}

async Task DeleteMyTask(string? id, HttpResponse response)
{
    using (ApplicationContext db = new ApplicationContext())
    {
        // получаем пользовател€ по id
        MyTask? MyTask = db.MyTasks.FirstOrDefault((t) => t.Id == id);
        // если пользователь найден, удал€ем его
        if (MyTask != null)
        {
            db.MyTasks.Remove(MyTask);
            db.SaveChanges();
            await response.WriteAsJsonAsync(MyTask);
        }

        // если не найден, отправл€ем статусный код и сообщение об ошибке
        else
        {
            response.StatusCode = 404;
            await response.WriteAsJsonAsync(new { message = "«адача не найдена" });
        }
    }
}

async Task CreateMyTask(HttpResponse response, HttpRequest request)
{
    try
    {
        // получаем данные пользовател€
        var myTask = await request.ReadFromJsonAsync<MyTask>();
        if (myTask != null)
        {
            // устанавливаем id дл€ нового пользовател€
            myTask.Id = Guid.NewGuid().ToString();
            // добавл€ем пользовател€ в список
            using (ApplicationContext db = new ApplicationContext())
            {
                db.MyTasks.Add(myTask);
                db.SaveChanges();
            }
            await response.WriteAsJsonAsync(myTask);
        }
        else
        {
            throw new Exception("Ќекорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Ќекорректные данные" });
    }
}

async Task UpdateMyTask(HttpResponse response, HttpRequest request)
{
    try
    {
        // получаем данные пользовател€
        MyTask? myTaskData = await request.ReadFromJsonAsync<MyTask>();
        if (myTaskData != null)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // получаем задачу по id
                var myTask = db.MyTasks.FirstOrDefault(t => t.Id == myTaskData.Id);
                // если пользователь найден, измен€ем его данные и отправл€ем обратно клиенту
                if (myTask != null)
                {
                    myTask.Heading = myTaskData.Heading;
                    myTask.Content = myTaskData.Content;
                    myTask.Date = myTaskData.Date;
                    myTask.Priority = myTaskData.Priority;
                    db.SaveChanges();
                    await response.WriteAsJsonAsync(myTask);
                }
                else
                {
                    response.StatusCode = 404;
                    await response.WriteAsJsonAsync(new { message = "ѕользователь не найден" });
                }
            }
        }
        else
        {
            throw new Exception("Ќекорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Ќекорректные данные" });
    }
}
