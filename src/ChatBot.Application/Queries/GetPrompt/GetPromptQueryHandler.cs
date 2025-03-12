using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Queries.GetPrompt;

internal class GetPromptQueryHandler(
    ILogger<GetPromptQueryHandler> logger,
    IPromptRepository promptRepository) 
    : IRequestHandler<GetPromptQuery, Prompt>
{
    private readonly ILogger _logger = logger;
    private readonly IReadPromptRepository _readPromptRepository = promptRepository;

    public async Task<Prompt> Handle(GetPromptQuery request, CancellationToken cancellationToken)
    {
        Prompt? prompt = await _readPromptRepository.GetAsync(request.PromptId, cancellationToken);
        
        if (prompt is null)
        {
            _logger.LogError(
                "Attempted to get prompt with id ({PromptId}) for owner ({OwnerId}), but prompt was not found.",
                request.PromptId,
                request.UserId);
            throw new PromptNotFoundException(request.PromptId, request.UserId);
        }

        if (prompt.OwnerId != request.UserId
            && prompt.OwnerId != Constants.SystemUser)
        {
            _logger.LogError(
                "Attempted to get prompt with id ({PromptId}) for user ({User}), but user is not authorized.",
                request.PromptId,
                request.UserId);
            throw new PromptAuthorizationException(request.PromptId, request.UserId);
        }
        
        _logger.LogInformation(
            "Getting prompt with id ({PromptId}) for owner ({Owner}).",
            request.PromptId,
            request.UserId);
        return prompt;
    }
}
