using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Commands.DeletePrompt;

internal class DeletePromptCommandHandler(IPromptRepository promptRepository)
    : IRequestHandler<DeletePromptCommand>
{
    private readonly IPromptRepository _promptRepository = promptRepository;
    
    public async Task Handle(DeletePromptCommand request, CancellationToken cancellationToken)
    {
        // TODO: Handle Getting the prompt and deleting it
        var prompt = await _promptRepository.GetAsync(request.PromptId, cancellationToken);
        
        if (prompt is null)
        {
            throw new PromptNotFoundException(request.PromptId, request.Username);
        }
        
        if (prompt.Owner != request.Username)
        {
            throw new PromptAuthorizationException(request.PromptId, request.Username);
        }
        
        await _promptRepository.DeleteAsync(prompt, cancellationToken);
    }
}
