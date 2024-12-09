using ChatBot.Api.Domain.Exceptions.PromptExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.ExceptionHandlers.PromptExceptionHandlers;

public class PromptDuplicateKeyExceptionHandler(ILogger<PromptDuplicateKeyExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<PromptDuplicateKeyExceptionHandler> _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not PromptDuplicateKeyException ex)
        {
            return false;
        }
        
        _logger.LogError(
            ex, 
            "Prompt belong to User ({Owner}) with key ({Key}) already exists.", 
            ex.Owner,
            ex.Key);
        
        var problemDetails = new ProblemDetails
        {
            Title = "Prompt Request Error",
            Detail = ex.Message,
            Status = StatusCodes.Status409Conflict,
            Extensions = new Dictionary<string, object?>()
            {
                ["key"] = ex.Key,
                ["owner"] = ex.Owner
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}