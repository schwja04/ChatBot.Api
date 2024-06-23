FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory inside the container
WORKDIR /src

# Copy the project file and restore the dependencies
COPY . .

# Restore the dependencies
RUN dotnet restore

# Build the solution in release mode
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish src/ChatBot.Api/ChatBot.Api.csproj -c Release -o /app/publish

# Use the official .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime

# Set the working directory inside the container
WORKDIR /app

# COPY the published output from the build stage
COPY --from=build-env /app/publish .

# Expose the port the application runs on
EXPOSE 5001

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ChatBot.Api.dll"]
