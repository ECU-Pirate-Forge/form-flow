
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

            return await response.Content.ReadFromJsonAsync<List<QuestionDefinition>>()
                ?? new List<QuestionDefinition>();
        }

        public async Task<(bool Success, string? Error)> CreateQuestionAsync(NewQuestion newQuestion)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("/api/questions", newQuestion);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                var body = await response.Content.ReadAsStringAsync();
                return (false, $"API ERROR: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Could not reach the server: {ex.Message}");
            }
        }
    }
}