using System.Text.RegularExpressions;
using TaskPlaner.Interfaces;

namespace TaskPlaner
{
    public class RoutingMiddleware
    {
        private readonly RequestDelegate _next;

        public RoutingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IDbWorker db)
        {
            var response = context.Response;
            var request = context.Request;
            var path = request.Path;
            //string expressionForNumber = "^/api/users/([0-9]+)$";   // если id представляет число

            // 2e752824-1657-4c7f-844b-6ec2e168e99c
            string expressionForGuid = @"^/api/tasks/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
            if (path == "/api/tasks" && request.Method == "GET")
            {
                await db.GetAllMyTasks(response);
            }
            else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
            {
                // получаем id из адреса url
                string? id = path.Value?.Split("/")[3];
                await db.GetMyTask(id, response);
            }
            else if (path == "/api/tasks" && request.Method == "POST")
            {
                await db.CreateMyTask(response, request);
            }
            else if (path == "/api/tasks" && request.Method == "PUT")
            {
                await db.UpdateMyTask(response, request);
            }
            else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
            {
                string? id = path.Value?.Split("/")[3];
                await db.DeleteMyTask(id, response);
            }
            else
            {
                response.ContentType = "text/html; charset=utf-8";
                await response.SendFileAsync("wwwroot/html/index.html");
            }
        }
        

    }
}
