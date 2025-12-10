using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITaskService
    {
        public Task<IEnumerable<Domain.Task>> GetAllTasksAsync();
        public Task<Domain.Task?> GetTaskByIdAsync(Guid id);
        public Task<Domain.Task> CreateTaskAsync(Domain.Task task);
        public Task<Domain.Task> UpdateTaskAsync(Domain.Task task);
        public Task DeleteTaskAsync(Guid id);
    }
}
