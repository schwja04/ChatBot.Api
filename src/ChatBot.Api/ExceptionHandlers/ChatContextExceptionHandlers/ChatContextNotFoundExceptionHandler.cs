using ChatBot.Api.Domain.Exceptions.ChatContextExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.ExceptionHandlers.ChatContextExceptionHandlers;

public class ChatContextNotFoundExceptionHandler(
    ILogger<ChatContextNotFoundExceptionHandler> logger)
    : IExceptionHandler
{
    private readonly ILogger<ChatContextNotFoundExceptionHandler> _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not ChatContextNotFoundException ex)
        {
            return false;
        }
        
        _logger.LogError(
            ex, 
            "ChatContext ({ContextId}) not found for User ({Username}).", 
            ex.ContextId,
            ex.Username);
        
        var problemDetails = new ProblemDetails
        {
            Title = "ChatContext Not Found",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound,
            Extensions = new Dictionary<string, object?>()
            {
                ["contextId"] = ex.ContextId,
                ["username"] = ex.Username
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}