using System.Collections.ObjectModel;
using ChatBot.Api.Domain.PromptEntity;
using MediatR;

namespace ChatBot.Api.Application.Queries;

public record GetPromptsQuery : IRequest<ReadOnlyCollection<Prompt>>
{
	public required string Username { get; init; }
}