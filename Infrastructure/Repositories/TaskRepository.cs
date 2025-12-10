using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskContext _taskContext;

        public TaskRepository(TaskContext taskContext)
        {
            _taskContext = taskContext;
        }
        public async Task<IEnumerable<Domain.Task>> GetAllTasksAsync()
        {
            return await _taskContext.Tasks.ToListAsync();
        }
        public async Task<Domain.Task?> GetTaskByIdAsync(Guid id)
        {
            return await _taskContext.Tasks.FindAsync(id);
        }
        public async Task<Domain.Task> CreateTaskAsync(Domain.Task task)
        {
            var entityEntry = await _taskContext.Tasks.AddAsync(task);
            await _taskContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
        public async Task<Domain.Task> UpdateTaskAsync(Domain.Task task)
        {
            var entityEntry = _taskContext.Tasks.Update(task);
            await _taskContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
        public async Task DeleteTaskAsync(Guid id)
        {
            var task = await _taskContext.Tasks.FindAsync(id);
            if (task != null)
            {
                _taskContext.Tasks.Remove(task);
                await _taskContext.SaveChangesAsync();
            }
        }
    }
}
