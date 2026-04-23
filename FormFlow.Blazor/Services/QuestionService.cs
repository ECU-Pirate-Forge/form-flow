
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
    }
}