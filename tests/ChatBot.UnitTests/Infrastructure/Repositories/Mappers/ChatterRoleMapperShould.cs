using ChatBot.Domain.ChatContextEntity;
using ChatBot.Infrastructure.Repositories.ExternalServices.ChatCompletion.Mappers;
using FluentAssertions;
using Microsoft.Extensions.AI;

namespace ChatBot.UnitTests.Infrastructure.Repositories.Mappers;

public class ChatterRoleMapperShould
{
    [Fact]
    public void ToChatterRole_ShouldMapChatterRoleToChatRole()
    {
        // Arrange
        var rolePairs = new List<(ChatterRole ChatterRole, ChatRole ChatRole)>()
        {
            (ChatterRole.User, ChatRole.User),
            (ChatterRole.Assistant, ChatRole.Assistant),
            (ChatterRole.System, ChatRole.System),
            (ChatterRole.Tool, ChatRole.Tool)
        };

        // Act && Assert
        foreach (var pair in rolePairs)
        {
            var result = pair.ChatterRole.ToLLMChatRole();
            result.Should().Be(pair.ChatRole);
        }
    }
    
}