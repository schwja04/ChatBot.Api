using AutoFixture;
using ChatBot.Api.Domain.PromptEntity;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Mappers;
using ChatBot.Api.Infrastructure.Repositories.Persistence.Mongo.Models;
using FluentAssertions;

namespace ChatBot.Api.UnitTests.Intrastructure.Repositories.Mappers;

public class PromptMapperShould
{
	private readonly IFixture _fixture;

	public PromptMapperShould()
	{
		_fixture = new Fixture();
	}

	[Fact]
	public void ToDal()
	{
		// Arrange
		Prompt prompt = Prompt.CreateNew(
			_fixture.Create<string>(),
			_fixture.Create<string>(),
			_fixture.Create<string>());

		// Act
		PromptDal promptDal = prompt.ToDal();

		// Assert
		prompt.Should().BeEquivalentTo(promptDal);
    }

    [Fact]
    public void ToDomain()
    {
		// Arrange
		PromptDal promptDal = _fixture.Create<PromptDal>();

        // Act
        Prompt prompt = promptDal.ToDomain();

        // Assert
        promptDal.Should().BeEquivalentTo(prompt);
    }
}
