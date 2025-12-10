using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<IEnumerable<Domain.Task>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllTasksAsync();
        }

        public async Task<Domain.Task?> GetTaskByIdAsync(Guid id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }

        public async Task<Domain.Task> CreateTaskAsync(Domain.Task task)
        {
            return await _taskRepository.CreateTaskAsync(task);
        }

        public async Task<Domain.Task> UpdateTaskAsync(Domain.Task task)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(task.Id);
            if (existingTask == null)
            {
                throw new Exception("Task not found");
            }

            // Update de eigenschappen van de bestaande taak
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.LastModifiedOn = DateTimeOffset.UtcNow;

            // Als de taak is voltooid, stel dan CompletedOn in
            if (task.Status == Domain.TaskStatus.Completed)
            {
                existingTask.CompletedOn = task.CompletedOn;
            }

            // Sla de bijgewerkte taak op in ITaskRepository
            return await _taskRepository.UpdateTaskAsync(existingTask);
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            await _taskRepository.DeleteTaskAsync(id);
        }
    }
}
