using System.ComponentModel;
using ChatBot.Domain.ChatContextEntity;
using Microsoft.Extensions.AI;

namespace ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;

internal static class ChatterRoleMapper
{
    public static ChatRole ToChatterRole(this ChatterRole role)
    {
        return role switch
        {
            ChatterRole.System => ChatRole.System,
            ChatterRole.Assistant => ChatRole.Assistant,
            ChatterRole.User => ChatRole.User,
            ChatterRole.Tool => ChatRole.Tool,
            _ => throw new InvalidEnumArgumentException(nameof(role), (int)role, typeof(ChatterRole))
        };
    }
}