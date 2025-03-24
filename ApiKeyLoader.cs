using System;
using System.IO;

namespace AI_Lawyer
{
    internal class ApiKeyLoader
    {
        private static readonly string KeyFile = "ApiKey.txt";

        public static string LoadApiKey()
        {
            try
            {
                return File.ReadAllText(KeyFile).Trim();
            }
            catch(Exception ex)
            {
                throw new Exception("Не удалось загрузить API-ключ. Проверьте файл", ex);
            }
        }
    }
}
