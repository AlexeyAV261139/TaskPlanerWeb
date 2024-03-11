using API.Contracts;
using Application;
using Microsoft.AspNetCore.Mvc;

namespace API
{
    [ApiController]
    [Route("[controller]")]
    public class MyTaskController : ControllerBase
    {
        private readonly MyTaskService _myTaskService;

        public MyTaskController(MyTaskService myTaskService)
        {
            _myTaskService = myTaskService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id)
        {

        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateMyTaskRequest request)
        {
            var myTask = new MyTask
            {
                //тут присвоим все необходимые свойства объекту
                //из полученного запроса
            };

            await _myTaskService.Create(myTask);
            return Ok();
        }

    }
}
