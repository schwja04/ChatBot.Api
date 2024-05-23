open-browser:
	open https://localhost:5001/swagger/index.html

run:
	dotnet run --project ./src/ChatBot.Api/ChatBot.Api.csproj

clean-build:
	dotnet clean && dotnet build
