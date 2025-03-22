using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

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
    }
}
