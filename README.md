# Overview
This project began as a way to explore how I would implement "memory" in a chatbot application.
The idea was to provide a way for the chatbot to remember things that the user had said, and then use that information to provide more contextually relevant responses.
Once reaching a point where I felt I had a good understanding of how to implement this, I decided to expand the project to include a more complete chatbot application.
- Custom prompts
- Frontend UI (Separate project, not public yet)

## Memory
The memory system comes in the form of a `ChatContext` that maintains the full chat history. 
This context is then passed to the OpenAI Completion API with each request.

### Storage Progression
1. An `InMemory` list of `ChatContext` objects that is stored in the `ChatContextInMemoryRepository` class. This allowed me to quickly iterate on the solution without making it overly complex.
2. A `Mongo` implementation of the repository was the next step. This allowed me to worry less about the storage and focus more on the chatbot functionality.
3. I then implemented an `EF Core` implementation of the repository. This was an exercise in learning the technology and seeing how easy it was to swap out the storage technology.
    - PostgresSQL
    - SqlServer

## Testing
### Unit Tests
These are the most basic tests and are used to test individual components of the application.
### Integration Tests
This is where I began stretching to get as much out of this project as possible.
#### Libraries
- `WebApplicationFactory` - This is used to spin up a test server that the tests can interact with.
- `TestContainers` - This is used to spin up a docker container with a database for the tests to interact with.
  - This coupled with `WebApplicationFactory` allowed me to test the various database implementations in a repeatable fashion.
- `XUnit` - The testing framework I used. The functionality of the ICollectionFixture shined here as I was able to reuse the same database in multiple test classes.
- `WireMock.Net` (Still in testing) - This is used to mock the OpenAI API. This will allow me to test the chatbot functionality without actually hitting the API. 
This would be more ideal than an NSubstitute mock of the OpenAIClient, as it would be a true test of the solution.

## Authentication
### Keycloak
- Keycloak was chosen as the identity provider because it is open-source and easy to host locally in a docker container.
This is beneficial as it fits well with the Dotnet Aspire orchestration for having a local development environment up quickly.
### Setup
- Either setup with a realm init file or manually configure the realm and client.
    - I've opted to configure the realm and client manually as it will not be done often.

## Future
- Redis Cache for Prompts to replace the in-memory cache
- Eventing
  - Use of a message broker
- Frontend Rewrite
  - Blazor or React
- Include an iac project for deploying the solution to Azure
- Build out CI/CD pipeline
- Token tracking
  - Track per user
  - Track per session
