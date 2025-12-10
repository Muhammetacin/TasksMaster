using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TasksMaster.DTOs;

namespace TasksMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetTasks")]
        public async Task<IEnumerable<TaskReadDto>> GetTasksAsync()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return _mapper.Map<IEnumerable<TaskReadDto>>(tasks);
        }

        [HttpGet("{id}", Name = "GetTaskById")]
        public async Task<ActionResult<TaskReadDto?>> GetTaskByIdAsync(Guid id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            var taskReadDto = _mapper.Map<TaskReadDto>(task);
            return taskReadDto == null ? NotFound() : Ok(taskReadDto);
        }

        [HttpPost(Name = "CreateTask")]
        public async Task<ActionResult<TaskReadDto>> CreateTaskAsync([FromBody] TaskCreateDto task)
        {
            var taskToCreate = _mapper.Map<Domain.Task>(task);
            var createdTask = await _taskService.CreateTaskAsync(taskToCreate);

            var taskReadDto = _mapper.Map<TaskReadDto>(createdTask);

            // Use the named route to generate the URL for the created resource
            return CreatedAtRoute(
                "GetTaskById", // route name defined on the GET by id endpoint
                new { id = taskReadDto.Id },
                taskReadDto
            );
        }

        [HttpPut("{id}", Name = "UpdateTask")]
        public async Task<ActionResult<TaskReadDto>> UpdateTaskAsync(Guid id, [FromBody] TaskUpdateDto taskUpdateDto)
        {
            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            _mapper.Map(taskUpdateDto, existingTask);

            var updatedTask = await _taskService.UpdateTaskAsync(existingTask);
            var taskReadDto = _mapper.Map<TaskReadDto>(updatedTask);
            return Ok(taskReadDto);
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
