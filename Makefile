open-browser:
	open https://localhost:5001/swagger/index.html

add-migration:
	dotnet ef migrations add $(name) --project ./efcore/ChatBot.Api.EntityFrameworkCore.$(databaseProvider)/ChatBot.Api.EntityFrameworkCore.$(databaseProvider).csproj --startup-project ./src/ChatBot.Api/ChatBot.Api.csproj --configuration $(environment)

update-database:
	dotnet ef database update --project ./efcore/ChatBot.Api.EntityFrameworkCore.$(databaseProvider)/ChatBot.Api.EntityFrameworkCore.$(databaseProvider).csproj --startup-project ./src/ChatBot.Api/ChatBot.Api.csproj --configuration $(environment)

run:
	dotnet run --project ./src/ChatBot.Api/ChatBot.Api.csproj

test:
	dotnet test ./tests/ChatBot.Api.UnitTests/ChatBot.Api.UnitTests.csproj

clean-build:
	dotnet clean && dotnet build
