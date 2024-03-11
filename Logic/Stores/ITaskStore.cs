namespace Logic.Stores
{
    public interface ITaskStore
    {
        Task<IReadOnlyList<MyTask>> GetByFilter(MyTaskFilter filter);

        Task<MyTask> GetById(Guid id);

        Task<MyTask> GetByNameAsync(string name);

        Task Add(MyTask task);
    }
}
