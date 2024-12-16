using ChatBot.Api.Domain.Exceptions.PromptExceptions;
using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Commands.CreatePrompt;

internal class CreatePromptCommandHandler(IPromptRepository promptRepository)
    : IRequestHandler<CreatePromptCommand, Prompt>
{
    private readonly IPromptRepository _promptRepository = promptRepository;

    public async Task<Prompt> Handle(
        CreatePromptCommand request, CancellationToken cancellationToken)
    {
        var existingPrompt = await _promptRepository.GetAsync(request.Owner, request.Key, cancellationToken);
        if (existingPrompt is not null)
        {
            throw new PromptDuplicateKeyException(request.Key, request.Owner);
        }
        
        var newPrompt = Prompt.CreateNew(request.Key, request.Value, request.Owner);

        await _promptRepository.CreateAsync(newPrompt, cancellationToken);

        return newPrompt;
    }
}
