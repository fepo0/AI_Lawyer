using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace AI_Lawyer.Data
{
    public class LawDatabase
    {
        // Подключение к БД
        private const string ConnectionString = "Server=localhost; Database=LawsDB; User ID=root; Password=12345;";

        public static List<string> GetLaws()
        {
            var laws = new List<string>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                // SQL запрос, для получения данных
                var command = new MySqlCommand("SELECT ArticleNumber, Title FROM Laws", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        laws.Add($"{reader.GetString(0)} - {reader.GetString(1)}");
                    }
                }
            }
            return laws;
        }

        public static List<string> SearchLaws(string caseDescription)
        {
            var matchedLaws = new List<string>();

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                var words = caseDescription
                    .Split(new[] { ' ', ',', '.', '!', '?', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 2)
                    .ToArray();

                if (words.Length == 0)
                {
                    return matchedLaws;
                }

                string query = "SELECT ArticleNumber, Title FROM Laws WHERE ";
                query += string.Join(" OR ", words.Select((w, i) => $"Content LIKE @word{i}"));

                var command = new MySqlCommand(query, connection);

                for (int i = 0; i < words.Length; i++)
                {
                    command.Parameters.AddWithValue($"@word{i}", $"%{words[i]}%");
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matchedLaws.Add($"{reader.GetString(0)} - {reader.GetString(1)}");
                    }
                }
            }
            return matchedLaws;
        }
    }
}
