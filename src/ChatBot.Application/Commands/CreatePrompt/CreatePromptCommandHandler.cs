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
        var existingPrompt = await _promptRepository.GetAsync(request.Owner, request.Key, cancellationToken);
        if (existingPrompt is not null)
        {
            _logger.LogError(
                "Attempted to create prompt with key ({Key}) for owner ({Owner}), but prompt with same key already exists.",
                request.Key,
                request.Owner);
            throw new PromptDuplicateKeyException(request.Key, request.Owner);
        }
        _logger.LogInformation(
            "Creating new prompt with key ({Key}) and value ({Value}) for owner ({Owner}).",
            request.Key,
            request.Value,
            request.Owner);
        var newPrompt = Prompt.CreateNew(request.Key, request.Value, request.Owner);

        await _promptRepository.CreateAsync(newPrompt, cancellationToken);

        return newPrompt;
    }
}
