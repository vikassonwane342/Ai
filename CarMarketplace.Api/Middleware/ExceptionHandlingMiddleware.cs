using System.Net;
using System.Text.Json;
using CarMarketplace.Api.Common;

namespace CarMarketplace.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing request.");
            await WriteResponseAsync(context, exception);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            BadRequestException => HttpStatusCode.BadRequest,
            UnauthorizedAppException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        var response = ApiResponse<object>.FailureResponse(
            statusCode == HttpStatusCode.InternalServerError ? "An unexpected error occurred." : exception.Message,
            statusCode == HttpStatusCode.InternalServerError ? ["Please contact support if the issue persists."] : []);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
