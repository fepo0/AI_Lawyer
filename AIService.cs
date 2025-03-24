using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AI_Lawyer
{
    internal class AIService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ApiUrl = "https://openrouter.ai/api/v1/chat/completions";
        private static readonly string ApiKey = ApiKeyLoader.LoadApiKey();

        public async Task<string> GetRecommendationsAsync(string caseDescription)
        {
            var requestData = new
            {
                model = "google/gemma-3-1b-it:free",
                messages = new[]
                {
                    new {role = "system", content = "Ты AI-адвокат, предоставляющий рекомендации по юредическим делам в Республике Беларусь."},
                    new {role = "user", content = caseDescription}
                },
                max_tokens = 150
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var response = await httpClient.PostAsync(ApiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка при обращении к API: {response.StatusCode}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseString);
            return responseObject.choices[0].message.content;
        }
    }
}
