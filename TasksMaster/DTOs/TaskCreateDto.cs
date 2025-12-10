namespace TasksMaster.DTOs
{
    public class TaskCreateDto
    {
        // Alleen de velden die de client mag aanleveren:
        public string Title { get; set; } // Meestal verplicht!
        public string? Description { get; set; } // Optioneel
    }
}
