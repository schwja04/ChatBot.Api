using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Commands.UpdatePrompt;

internal class UpdatePromptCommandHandler(IPromptRepository promptRepository)
	: IRequestHandler<UpdatePromptCommand>
{
    private readonly IPromptRepository _promptRepository = promptRepository;

    public async Task Handle(UpdatePromptCommand request, CancellationToken cancellationToken)
    {
        Prompt? promptById = await _promptRepository.GetAsync(request.PromptId, cancellationToken);
        if (promptById is null)
        {
            throw new PromptNotFoundException(request.PromptId, request.Owner);
        }
        if (promptById.Owner != request.Owner)
        {
            throw new PromptAuthorizationException(request.PromptId, request.Owner);
        }
        
        Prompt? promptByKey = await _promptRepository.GetAsync(request.Owner, request.Key, cancellationToken);
        if (promptByKey is not null && promptByKey.PromptId != request.PromptId)
        {
            throw new PromptDuplicateKeyException(request.Key, request.Owner);
        }
        
        promptById.UpdateKey(request.Key);
        promptById.UpdateValue(request.Value);
        
		await _promptRepository.UpdateAsync(promptById, cancellationToken);
    }
}
