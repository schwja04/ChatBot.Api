open-browser:
	open https://localhost:5001/swagger/index.html

run:
	dotnet run --project ./src/ChatBot.Api/ChatBot.Api.csproj

test:
	dotnet test ./tests/ChatBot.Api.UnitTests/ChatBot.Api.UnitTests.csproj

clean-build:
	dotnet clean && dotnet build
