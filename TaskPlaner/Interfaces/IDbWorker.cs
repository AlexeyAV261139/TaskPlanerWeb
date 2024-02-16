namespace TaskPlaner.Interfaces
{
    public interface IDbWorker
    {
        Task CreateMyTask(HttpResponse response, HttpRequest request);
        Task DeleteMyTask(string? id, HttpResponse response);
        Task GetAllMyTasks(HttpResponse response);
        Task GetMyTask(string? id, HttpResponse response);
        Task UpdateMyTask(HttpResponse response, HttpRequest request);
    }
}