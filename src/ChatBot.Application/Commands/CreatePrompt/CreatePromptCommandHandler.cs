using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Commands.CreatePrompt;

internal class CreatePromptCommandHandler(
    ILogger<CreatePromptCommandHandler> logger,
    IPromptRepository promptRepository)
    : IRequestHandler<CreatePromptCommand, Prompt>
{
    private readonly ILogger _logger = logger;
    private readonly IPromptRepository _promptRepository = promptRepository;

    public async Task<Prompt> Handle(
        CreatePromptCommand request, CancellationToken cancellationToken)
    {
        var existingPrompt = await _promptRepository.GetAsync(request.OwnerId, request.Key, cancellationToken);
        if (existingPrompt is not null)
        {
            _logger.LogError(
                "Attempted to create prompt with key ({Key}) for owner ({OwnerId}), but prompt with same key already exists.",
                request.Key,
                request.OwnerId);
            throw new PromptDuplicateKeyException(request.Key, request.OwnerId);
        }
        _logger.LogInformation(
            "Creating new prompt with key ({Key}) and value ({Value}) for owner ({OwnerId}).",
            request.Key,
            request.Value,
            request.OwnerId);
        var newPrompt = Prompt.CreateNew(request.Key, request.Value, request.OwnerId);

        await _promptRepository.CreateAsync(newPrompt, cancellationToken);

        return newPrompt;
    }
}
