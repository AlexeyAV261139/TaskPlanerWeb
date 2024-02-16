using TaskPlaner.Interfaces;

namespace TaskPlaner
{
    public class DbWorker : IDbWorker
    {
        // получение всех пользователей
        public async Task GetAllMyTasks(HttpResponse response)
        {
            using ApplicationContext db = new ApplicationContext();
            {
                await response.WriteAsJsonAsync(db.MyTasks);
            }
        }
        // получение одного пользователя по id
        public async Task GetMyTask(string? id, HttpResponse response)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // получаем пользователя по id
                MyTask? myTask = db.MyTasks.FirstOrDefault((t) => t.Id == id);
                // если пользователь найден, отправляем его
                if (myTask != null)
                    await response.WriteAsJsonAsync(myTask);
                // если не найден, отправляем статусный код и сообщение об ошибке
                else
                {
                    response.StatusCode = 404;
                    await response.WriteAsJsonAsync(new { message = "Задача не найдена" });
                }
            }
        }

        public async Task DeleteMyTask(string? id, HttpResponse response)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // получаем пользователя по id
                MyTask? MyTask = db.MyTasks.FirstOrDefault((t) => t.Id == id);
                // если пользователь найден, удаляем его
                if (MyTask != null)
                {
                    db.MyTasks.Remove(MyTask);
                    db.SaveChanges();
                    await response.WriteAsJsonAsync(MyTask);
                }

                // если не найден, отправляем статусный код и сообщение об ошибке
                else
                {
                    response.StatusCode = 404;
                    await response.WriteAsJsonAsync(new { message = "Задача не найдена" });
                }
            }
        }

        public async Task CreateMyTask(HttpResponse response, HttpRequest request)
        {
            try
            {
                // получаем данные пользователя
                var myTask = await request.ReadFromJsonAsync<MyTask>();
                if (myTask != null)
                {
                    // устанавливаем id для нового пользователя
                    myTask.Id = Guid.NewGuid().ToString();
                    // добавляем пользователя в список
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.MyTasks.Add(myTask);
                        db.SaveChanges();
                    }
                    await response.WriteAsJsonAsync(myTask);
                }
                else
                {
                    throw new Exception("Некорректные данные");
                }
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
            }
        }

        public async Task UpdateMyTask(HttpResponse response, HttpRequest request)
        {
            try
            {
                // получаем данные пользователя
                MyTask? myTaskData = await request.ReadFromJsonAsync<MyTask>();
                if (myTaskData != null)
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        // получаем задачу по id
                        var myTask = db.MyTasks.FirstOrDefault(t => t.Id == myTaskData.Id);
                        // если пользователь найден, изменяем его данные и отправляем обратно клиенту
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
                            await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
                        }
                    }
                }
                else
                {
                    throw new Exception("Некорректные данные");
                }
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
            }
        }
    }
}
