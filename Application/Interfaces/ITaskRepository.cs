using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Domain.Task>> GetAllTasksAsync();
        Task<Domain.Task?> GetTaskByIdAsync(Guid id);
        Task<Domain.Task> CreateTaskAsync(Domain.Task task);
        Task<Domain.Task> UpdateTaskAsync(Domain.Task task);
        Task DeleteTaskAsync(Guid id);
    }
}
