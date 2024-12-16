using System.Collections.ObjectModel;
using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries.GetManyPrompts;

public record GetManyPromptsQuery : IRequest<ReadOnlyCollection<Prompt>>
{
	public required string Username { get; init; }
	
	public required bool IncludeSystemPrompts { get; init; }
}