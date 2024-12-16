using ChatBot.Domain.Exceptions.PromptExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.ExceptionHandlers.PromptExceptionHandlers;

public class PromptNotFoundExceptionHandler(ILogger<PromptNotFoundExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<PromptNotFoundExceptionHandler> _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not PromptNotFoundException ex)
        {
            return false;
        }
        
        _logger.LogError(
            ex,
            "Prompt ({PromptId}) not found for user ({owner}).",
            ex.PromptId,
            ex.Owner);

        var problemDetails = new ProblemDetails
        {
            Title = "Prompt Not Found Error",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound,
            Extensions = new Dictionary<string, object?>()
            {
                ["promptId"] = ex.PromptId,
                ["username"] = ex.Owner
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}