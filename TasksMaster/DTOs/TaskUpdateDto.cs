namespace TasksMaster.DTOs
{
    public class TaskUpdateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        public TaskStatus Status { get; set; }
        public DateTimeOffset? CompletedOn { get; set; }
    }
}
