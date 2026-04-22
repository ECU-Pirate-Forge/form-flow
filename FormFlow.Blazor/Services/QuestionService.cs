
using FormFlow.Data.Models;

namespace FormFlow.Blazor.Services
{
    public class QuestionService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<IEnumerable<QuestionDefinition>?> GetAllQuestionsAsync()
        {
            IEnumerable<QuestionDefinition>? questions = await _httpClient
                .GetFromJsonAsync<IEnumerable<QuestionDefinition>>("questions");

            return questions;
        }


    }
}