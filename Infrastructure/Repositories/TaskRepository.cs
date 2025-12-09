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
    }
}
