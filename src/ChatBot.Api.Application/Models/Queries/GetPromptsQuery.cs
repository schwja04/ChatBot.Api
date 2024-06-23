using System.Collections.ObjectModel;
using MediatR;

namespace ChatBot.Api.Application.Models.Queries;

public record GetPromptsQuery : IRequest<ReadOnlyCollection<Prompt>>
{
	public required string Username { get; init; }
}