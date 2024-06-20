using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models;
using ChatBot.Api.Application.Models.Queries;
using MediatR;

namespace ChatBot.Api.Application;

internal class GetPromptQueryHandler : IRequestHandler<GetPromptQuery, GetPromptQueryResponse>
{
    private readonly IReadPromptRepository _readPromptRepository;

    public GetPromptQueryHandler(IPromptRepository promptRepository)
    {
        _readPromptRepository = promptRepository;
    }

    public async Task<GetPromptQueryResponse> Handle(GetPromptQuery request, CancellationToken cancellationToken)
    {
        Prompt? prompt = null;
        if (request.PromptId.HasValue)
        {
            prompt = await _readPromptRepository.GetAsync(request.Username, request.PromptId.Value, cancellationToken);
            return new GetPromptQueryResponse
            {
                Prompt = prompt,
            };
        }

        prompt = await _readPromptRepository.GetAsync(request.Username, request.PromptKey!, cancellationToken);

        return new GetPromptQueryResponse
        {
            Prompt = prompt,
        };
    }
}
