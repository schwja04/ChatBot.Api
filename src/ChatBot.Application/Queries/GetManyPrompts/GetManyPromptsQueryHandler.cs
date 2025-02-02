using System.Collections.ObjectModel;
using ChatBot.Domain.PromptEntity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Queries.GetManyPrompts;

internal class GetManyPromptsQueryHandler(
	ILogger<GetManyPromptsQueryHandler> logger,
	IPromptRepository promptRepository)
	: IRequestHandler<GetManyPromptsQuery, ReadOnlyCollection<Prompt>>
{
	private readonly ILogger _logger = logger;
	private readonly IReadPromptRepository _readPromptRepository = promptRepository;

    public async Task<ReadOnlyCollection<Prompt>> Handle(GetManyPromptsQuery request, CancellationToken cancellationToken)
    {
	    _logger.LogInformation(
		    "Getting prompts for user ({Username}).",
		    request.Username);
	    var userPrompts = await _readPromptRepository.GetManyAsync(request.Username, cancellationToken);
	    
	    if (!request.IncludeSystemPrompts)
	    {
		    return userPrompts;
	    }
	    
	    _logger.LogInformation(
		    "Getting system prompts for user ({Username}).",
		    request.Username);
	    var systemPrompts = await _readPromptRepository.GetManyAsync(Constants.SystemUser, cancellationToken);
	    
	    var allPrompts = new List<Prompt>(userPrompts);
	    allPrompts.AddRange(systemPrompts);
	    return allPrompts.AsReadOnly();
    }
}

