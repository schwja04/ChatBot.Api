using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Application.Models.Queries;
using MediatR;

namespace ChatBot.Api.Application.QueryHandlers;

internal class GetPromptQueryHandler : IRequestHandler<GetPromptQuery, Prompt?>
{
    private readonly IReadPromptRepository _readPromptRepository;

    public GetPromptQueryHandler(IPromptRepository promptRepository)
    {
        _readPromptRepository = promptRepository;
    }

    public async Task<Prompt?> Handle(GetPromptQuery request, CancellationToken cancellationToken)
    {
        if (request.PromptId.HasValue)
        {
            return await _readPromptRepository.GetAsync(request.Username, request.PromptId.Value, cancellationToken);
        }

        return await _readPromptRepository.GetAsync(request.Username, request.PromptKey!, cancellationToken);
    }
}
