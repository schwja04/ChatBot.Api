using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Commands.UpdatePrompt;

internal class UpdatePromptCommandHandler(
    ILogger<UpdatePromptCommandHandler> logger,
    IPromptRepository promptRepository)
	: IRequestHandler<UpdatePromptCommand>
{
    private readonly ILogger _logger = logger;
    private readonly IPromptRepository _promptRepository = promptRepository;

    public async Task Handle(UpdatePromptCommand request, CancellationToken cancellationToken)
    {
        Prompt? promptById = await _promptRepository.GetAsync(request.PromptId, cancellationToken);
        if (promptById is null)
        {
            _logger.LogError(
                "Attempted to update prompt with id ({PromptId}) for user ({UserId}), but prompt was not found.",
                request.PromptId,
                request.UserId);
            throw new PromptNotFoundException(request.PromptId, request.UserId);
        }
        if (promptById.OwnerId != request.UserId)
        {
            _logger.LogError(
                "Attempted to update prompt with id ({PromptId}) for user ({UserId}), but user is not authorized.",
                request.PromptId,
                request.UserId);
            throw new PromptAuthorizationException(request.PromptId, request.UserId);
        }
        
        Prompt? promptByKey = await _promptRepository.GetAsync(request.UserId, request.Key, cancellationToken);
        if (promptByKey is not null && promptByKey.PromptId != request.PromptId)
        {
            _logger.LogError(
                "Attempted to update prompt with key ({Key}) for owner ({Owner}), but a prompt with same key already exists.",
                request.Key,
                request.UserId);
            throw new PromptDuplicateKeyException(request.Key, request.UserId);
        }
        
        promptById.UpdateKey(request.Key);
        promptById.UpdateValue(request.Value);
        
        _logger.LogInformation(
            "Updating prompt with id ({PromptId}) for owner ({Owner}).",
            request.PromptId,
            request.UserId);
		await _promptRepository.UpdateAsync(promptById, cancellationToken);
    }
}
