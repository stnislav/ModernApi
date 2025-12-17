using ModernApi.Exceptions;

namespace ModernApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning(ex, "Response already started, cannot write error body. TraceId={TraceId}", context.TraceIdentifier);
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        _logger.LogError(exception, "Unhandled exception. StatusCode={StatusCode} TraceId={TraceId}", statusCode, context.TraceIdentifier);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var message =
            _env.IsDevelopment() || statusCode != StatusCodes.Status500InternalServerError
                ? exception.Message
                : "An unexpected error occurred.";

        var response = new { error = message, traceId = context.TraceIdentifier };
        return context.Response.WriteAsJsonAsync(response);
    }
}
