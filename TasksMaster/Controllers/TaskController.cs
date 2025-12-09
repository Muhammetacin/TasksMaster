using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TasksMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet(Name = "GetTasks")]
        public async Task<IEnumerable<Domain.Task>> GetTasksAsync()
        {
             return await _taskService.GetAllTasksAsync();
        }
    }
}
