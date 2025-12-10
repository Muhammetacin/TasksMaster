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

        [HttpGet("{id}", Name = "GetTaskById")]
        public async Task<ActionResult<Domain.Task?>> GetTaskByIdAsync(Guid id)
        {
            return await _taskService.GetTaskByIdAsync(id);
        }

        [HttpPost(Name = "CreateTask")]
        public async Task<ActionResult<Domain.Task>> CreateTaskAsync([FromBody] Domain.Task task)
        {
            var createdTask = await _taskService.CreateTaskAsync(task);

            // Zorgt voor: HTTP 201 Created + de Location header (URL naar de zojuist gecreëerde Task)
            return CreatedAtAction(
                nameof(GetTaskByIdAsync), // Naam van de GET-methode voor één Task
                new { id = createdTask.Id }, // Route parameters om de URL te bouwen
                createdTask // De Task die we als body terugsturen
            );
        }

        [HttpPut("{id}", Name = "UpdateTask")]
        public async Task<ActionResult<Domain.Task>> UpdateTaskAsync(Guid id, [FromBody] Domain.Task task)
        {
            // Controle: Zorg ervoor dat de ID in de URL en de body overeenkomen
            if (id != task.Id)
            {
                // Geeft aan dat het verzoek conflicterende informatie bevat
                return BadRequest("Task ID in the URL must match the ID in the body.");
            }

            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            var updatedTask = await _taskService.UpdateTaskAsync(existingTask);
            return Ok(updatedTask);
        }

        [HttpDelete("{id}", Name = "DeleteTask")]
        public async Task<IActionResult> DeleteTaskAsync(Guid id)
        {
            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
