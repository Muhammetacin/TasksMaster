namespace TasksMaster.DTOs
{
    public class TaskReadDto
    {
        // Geen 'init' of standaardwaarden! Ze worden gevuld vanuit de Domain.Task
        public Guid Id { get; set; } 
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public TaskStatus Status { get; set; }
        public DateTimeOffset? CompletedOn { get; set; }
    }
}
