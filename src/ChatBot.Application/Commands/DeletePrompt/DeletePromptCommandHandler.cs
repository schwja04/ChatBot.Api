using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Commands.DeletePrompt;

internal class DeletePromptCommandHandler(
    ILogger<DeletePromptCommandHandler> logger,
    IPromptRepository promptRepository)
    : IRequestHandler<DeletePromptCommand>
{
    private readonly ILogger _logger = logger;
    private readonly IPromptRepository _promptRepository = promptRepository;
    
    public async Task Handle(DeletePromptCommand request, CancellationToken cancellationToken)
    {
        var prompt = await _promptRepository.GetAsync(request.PromptId, cancellationToken);
        
        if (prompt is null)
        {
            _logger.LogError(
                "Attempted to delete prompt with id ({PromptId}) for owner ({Owner}), but prompt was not found.",
                request.PromptId,
                request.UserId);
            throw new PromptNotFoundException(request.PromptId, request.UserId);
        }
        
        if (prompt.OwnerId != request.UserId)
        {
            _logger.LogError(
                "Attempted to delete prompt with id ({PromptId}) for user ({User}), but user is not authorized.",
                request.PromptId,
                request.UserId);
            throw new PromptAuthorizationException(request.PromptId, request.UserId);
        }
        
        _logger.LogInformation(
            "Deleting prompt with id ({PromptId}) for owner ({Owner}).",
            request.PromptId,
            request.UserId);
        await _promptRepository.DeleteAsync(prompt, cancellationToken);
    }
}
