using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AI_Lawyer.Services
{
    internal class OpenAIService
    {
        private readonly string ApiKey;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private static readonly HttpClient client = new HttpClient();

        public OpenAIService()
        {
            try
            {
                ApiKey = File.ReadAllText("ApiKey.txt").Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка: не удалось прочитать API-ключ. Проверьте файл.", ex);
            }
        }
        
        public async Task<string> GetLegalAdviceAsync(string caseDescription)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new {role = "system", content = "Ты опытный адвокат. Дай юридические рекомендации."},
                    new {role = "user", content = caseDescription }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            var jsonRequest = JsonConvert.SerializeObject(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            request.Headers.Add("Authorization", $"Bearer {ApiKey}");
            request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return $"Ошибка API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            dynamic parsedResponse = JsonConvert.DeserializeObject(jsonResponse);
            if (parsedResponse?.choices != null && parsedResponse.choices.Count > 0)
            {
                return parsedResponse.choices[0].message.content?.ToString() ?? "Ошибка: пустой ответ от AI";
            }
            else
            {
                return "Ошибка: OpenAI не вернул ответ.";
            }
            var responseContent = await response.Content.ReadAsStringAsync(); //
            Console.WriteLine("Ответ OpenAI: " + responseContent); //
            return parsedResponse?.choices[0]?.message?.content?.ToString();
        }
    }
}
