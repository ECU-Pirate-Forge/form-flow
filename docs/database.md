# Initial Database Reference

# Name of DB File:

    formflow.db

# Program.cs so Far:

    Lns 12-16:
     describe the LiteDB shared service. We create a singleton that uses a factory Function "(sp => ...)". This function retrieves the connection string from appsettings.json.

    # Questions Collection Access

    The backend exposes the LiteDB`questions` collection through a repository:

    - Repository:`FormFlow.Backend.Repositories.QuestionRepository`
    - Interface: `FormFlow.Backend.Repositories.IQuestionRepository`
    - Collection type: `ILiteCollection<QuestionDefinition>`
    - Collection name: `questions`

    The repository is registered in dependency injection as a singleton and can be injected into endpoints or services.

    Example:

    ```csharp
    public class SomeService
    {
        private readonly IQuestionRepository _questionRepository;

    public SomeService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
    }

   ```

# Repositories

## SurveyRepository

The SurveyRepository provides access to the LiteDB "surveys" collection.
It exposes a strongly typed ILiteCollection `<SurveyDefinition>` and is registered
in dependency injection as ISurveyRepository. This allows endpoints and services
to store and retrieve survey documents in a consistent and structured way.
