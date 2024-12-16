using ChatBot.Domain.Exceptions.PromptExceptions;
using ChatBot.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Application.Queries.GetPrompt;

internal class GetPromptQueryHandler(IPromptRepository promptRepository) 
    : IRequestHandler<GetPromptQuery, Prompt>
{
    private readonly IReadPromptRepository _readPromptRepository = promptRepository;

    public async Task<Prompt> Handle(GetPromptQuery request, CancellationToken cancellationToken)
    {
        Prompt? prompt = await _readPromptRepository.GetAsync(request.PromptId, cancellationToken);
        
        if (prompt is null)
        {
            throw new PromptNotFoundException(request.PromptId, request.Username);
        }

        if (!string.Equals(prompt.Owner, request.Username, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(prompt.Owner, Constants.SystemUser, StringComparison.OrdinalIgnoreCase))
        {
            throw new PromptAuthorizationException(request.PromptId, request.Username);
        }

        return prompt;
    }
}
