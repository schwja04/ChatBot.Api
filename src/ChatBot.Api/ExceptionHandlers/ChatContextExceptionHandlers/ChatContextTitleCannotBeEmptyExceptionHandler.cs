using ChatBot.Domain.Exceptions.ChatContextExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.ExceptionHandlers.ChatContextExceptionHandlers;

public class ChatContextTitleCannotBeEmptyExceptionHandler(
    ILogger<ChatContextTitleCannotBeEmptyExceptionHandler> logger)
    : IExceptionHandler
{
    private readonly ILogger<ChatContextTitleCannotBeEmptyExceptionHandler> _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ChatContextTitleCannotBeEmptyException ex)
        {
            return false;
        }
        
        _logger.LogError(
            ex, 
            "User ({Owner}) attempted to set ChatContext ({ContextId}) title to empty.", 
            ex.Owner,
            ex.ContextId);

        var problemDetails = new ProblemDetails
        {
            Title = "ChatContext Request Error",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Extensions = new Dictionary<string, object?>()
            {
                ["contextId"] = ex.ContextId,
                ["owner"] = ex.Owner
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}