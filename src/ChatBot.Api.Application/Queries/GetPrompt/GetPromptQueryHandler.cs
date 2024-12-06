using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetPrompt;

internal class GetPromptQueryHandler(IPromptRepository promptRepository) 
    : IRequestHandler<GetPromptQuery, Prompt?>
{
    private readonly IReadPromptRepository _readPromptRepository = promptRepository;

    public async Task<Prompt?> Handle(GetPromptQuery request, CancellationToken cancellationToken)
    {
        if (request.PromptId.HasValue)
        {
            return await _readPromptRepository.GetAsync(request.Username, request.PromptId.Value, cancellationToken);
        }

        return await _readPromptRepository.GetAsync(request.Username, request.PromptKey!, cancellationToken);
    }
}
