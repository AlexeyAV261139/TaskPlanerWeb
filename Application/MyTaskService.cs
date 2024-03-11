using Logic.Stores;

namespace Application
{
    public class MyTaskService
    {
        private readonly ITaskStore _taskStore;

        public MyTaskService(ITaskStore taskStore)
        {
            _taskStore = taskStore;
        }

        public async Task Get(Guid id)
        {
            await _taskStore.GetById(id);
        }

        public async Task Update(MyTask myTask)
        {

        }

        public async Task Get()
        {

        }

        public async Task Create(MyTask myTask)
        {

            // тут можно добавить логирование
            var existedOrder = await _taskStore.GetByNameAsync(myTask.Heading);

            if (existedOrder != null)
            {
                throw new Exception();
            }
            await _taskStore.Add(myTask);
        }
    }
}
