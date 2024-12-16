using ChatBot.Domain.Exceptions.ChatContextExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.ExceptionHandlers.ChatContextExceptionHandlers;

public class ChatContextAuthorizationExceptionHandler(
    ILogger<ChatContextAuthorizationExceptionHandler> logger) 
    : IExceptionHandler
{
    private readonly ILogger<ChatContextAuthorizationExceptionHandler> _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ChatContextAuthorizationException ex)
        {
            return false;
        }
        
        _logger.LogError(
            ex, 
            "User ({UnauthorizedUser}) is not authorized to access ChatContext ({ContextId}).", 
            ex.UnauthorizedUser, 
            ex.ContextId);
        
        var problemDetails = new ProblemDetails
        {
            Title = "ChatContext Authorization Error",
            Detail = ex.Message,
            Status = StatusCodes.Status401Unauthorized,
            Extensions = new Dictionary<string, object?>()
            {
                ["contextId"] = ex.ContextId,
                ["unauthorizedUser"] = ex.UnauthorizedUser
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}