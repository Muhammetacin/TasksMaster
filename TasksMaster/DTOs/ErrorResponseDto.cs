namespace TasksMaster.DTOs
{
    public class ErrorResponseDto
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
        public string TraceId { get; set; }
    }
}
