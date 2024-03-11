using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Stores
{
    public interface ITaskStore
    {
        Task<IReadOnlyList<MyTask>> GetByFilter(OrderFilter filter);

        Task<MyTask> GetById(Guid id);

        Task<MyTask> GetByNameAsync(string name);

        Task Add(MyTask task);
    }
}
