open-browser:
	open https://localhost:5001/swagger/index.html

add-migration:
	dotnet ef migrations add $(name) --project ./efcore/ChatBot.Infrastructure.EntityFrameworkCore.$(databaseProvider)/ChatBot.Infrastructure.EntityFrameworkCore.$(databaseProvider).csproj --startup-project ./src/ChatBot.Api/ChatBot.Api.csproj --configuration $(environment)

update-database:
	dotnet ef database update --project ./efcore/ChatBot.Infrastructure.EntityFrameworkCore.$(databaseProvider)/ChatBot.Infrastructure.EntityFrameworkCore.$(databaseProvider).csproj --startup-project ./src/ChatBot.Api/ChatBot.Api.csproj -- --environment $(environment)

run:
	dotnet run --project ./src/ChatBot.Api/ChatBot.Api.csproj

run-aspire:
	dotnet run --project ./aspire/ChatBot.Aspire.AppHost/ChatBot.Aspire.AppHost.csproj

test:
	dotnet test ./tests/ChatBot.UnitTests/ChatBot.UnitTests.csproj
	dotnet test ./tests/ChatBot.Api.IntegrationTests/ChatBot.Api.IntegrationTests.csproj

clean-build:
	dotnet clean && dotnet build
