using System;

namespace Domain
{
    public class Task
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset CreatedOn { get; init; } = DateTimeOffset.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset LastModifiedOn { get; set; } = DateTimeOffset.UtcNow;
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public DateTimeOffset? CompletedOn { get; set; }
    }
}
