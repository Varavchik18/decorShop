using System.Net;
using Newtonsoft.Json;
using DecorStore.Domain.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainValidationException ex)
        {
            _logger.LogError($"Validation errors: {string.Join(", ", ex.ErrorCodes)}");
            var errorCodes = ex.ErrorCodes.Select(code => code.ToString());
            await HandleExceptionAsync(context, HttpStatusCode.PreconditionFailed, new { message = ex.Message, errorCodes });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Data}, {ex.Message}, {ex.InnerException}, {ex.Source}");
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, new { message = ex.Message, ex.Data });
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, object response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}
