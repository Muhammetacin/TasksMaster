using System.Net;
using System.Text.Json;
using TasksMaster.DTOs;

namespace TasksMaster.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Hier gaat het verzoek door de rest van de app

                // NIEUW: Controleer de statuscode nadat het verzoek is afgehandeld
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                    context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("Autorisatie fout gedetecteerd: {StatusCode} op pad {Path}",
                        context.Response.StatusCode, context.Request.Path);
                }
            }
            catch (Exception ex)
            {
                // Dit blijft voor echte crashes (500 errors)
                _logger.LogError(ex, "Een onverwachte fout is opgetreden.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message); // Log naar console/bestand
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ErrorResponseDto
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,
                // Toon stacktrace alleen in development mode voor veiligheid
                Details = _env.IsDevelopment() ? ex.StackTrace?.ToString() : "Interne serverfout.",
                TraceId = context.TraceIdentifier
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
