using ChatBot.Domain.Exceptions.PromptExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Api.ExceptionHandlers.PromptExceptionHandlers;

public class PromptAuthorizationExceptionHandler(ILogger<PromptAuthorizationExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<PromptAuthorizationExceptionHandler> _logger = logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        // If the exception is an ApplicationException, we can handle it
        if (exception is not PromptAuthorizationException ex)
        {
            return false;
        }
        
        _logger.LogError(
            ex, 
            "User ({UnauthorizedUser}) is not authorized to access Prompt ({PromptId}).", 
            ex.UnauthorizedUserId, 
            ex.PromptId);
        
        var problemDetails = new ProblemDetails
        {
            Title = "Prompt Authorization Error",
            Detail = ex.Message,
            Status = StatusCodes.Status401Unauthorized,
            Extensions = new Dictionary<string, object?>()
            {
                ["promptId"] = ex.PromptId,
                ["unauthorizedUser"] = ex.UnauthorizedUserId
            }
        };

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}