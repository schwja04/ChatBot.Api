using MediatR;

using ChatBot.Api.Application.Abstractions.Repositories;
using ChatBot.Api.Application.Models.Queries;

namespace ChatBot.Api.Application.Handlers.QueryHandlers;

internal class GetPromptsQueryHandler : IRequestHandler<GetPromptsQuery, GetPromptsQueryResponse>
{
	private readonly IReadPromptRepository _readPromptRepository;

	public GetPromptsQueryHandler(IPromptRepository promptRepository)
	{
		_readPromptRepository = promptRepository;
	}

    public async Task<GetPromptsQueryResponse> Handle(GetPromptsQuery request, CancellationToken cancellationToken)
    {
		var prompts = await _readPromptRepository.GetManyAsync(request.Username, cancellationToken);

		return new GetPromptsQueryResponse
		{
			Prompts = prompts
		};
    }
}

