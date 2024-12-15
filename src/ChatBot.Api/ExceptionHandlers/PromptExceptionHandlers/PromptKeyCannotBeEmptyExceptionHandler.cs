using ChatBot.Api.Domain.Exceptions.PromptExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.ExceptionHandlers.PromptExceptionHandlers;

public class PromptKeyCannotBeEmptyExceptionHandler(ILogger<PromptKeyCannotBeEmptyExceptionHandler> logger) 
    : IExceptionHandler
{
    private readonly ILogger<PromptKeyCannotBeEmptyExceptionHandler> _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not PromptKeyCannotBeEmptyException ex)
        {
            return false;
        }
        
        _logger.LogError(
            ex, 
            "User ({Owner}) attempted to set Prompt ({PromptId}) to empty.", 
            ex.Owner,
            ex.PromptId);

        var problemDetails = new ProblemDetails
        {
            Title = "Prompt Request Error",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Extensions = new Dictionary<string, object?>()
            {
                ["promptId"] = ex.PromptId,
                ["owner"] = ex.Owner
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}