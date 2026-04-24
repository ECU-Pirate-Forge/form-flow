
using FormFlow.Data.Models;

namespace FormFlow.Blazor.Services
{
    public class QuestionService(HttpClient httpClient)
    {

        public async Task<List<QuestionDefinition>?> GetAllQuestionsAsync()
        {
            var response = await httpClient.GetAsync("/api/questions");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API ERROR: {(int)response.StatusCode} {response.ReasonPhrase}");
                Console.WriteLine($"Body: {errorContent}");

                return new List<QuestionDefinition>();
            }

            return await httpClient.GetFromJsonAsync<List<QuestionDefinition>>("api/questions")
                ?? new List<QuestionDefinition>();
        }

        public async Task<(bool Success, string? Error)> CreateQuestionAsync(NewQuestion newQuestion)
        {
            var question = new QuestionDefinition
            {
                Id = Guid.NewGuid(),
                Key = newQuestion.Key,
                Label = newQuestion.Label,
                Type = newQuestion.Type,
                Required = newQuestion.Required,
                Placeholder = newQuestion.Placeholder,
                DefaultValue = newQuestion.DefaultValue,
                HelpText = newQuestion.HelpText,
                Options = newQuestion.Options
            };

            var response = await httpClient.PostAsJsonAsync("/api/questions", question);

            if (response.IsSuccessStatusCode)
            {
               return (true, null);
            }
            var body = await response.Content.ReadAsStringAsync();
            return (false, $"API ERROR: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }
}