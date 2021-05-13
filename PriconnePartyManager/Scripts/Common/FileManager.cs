using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using PriconnePartyManager.Scripts.DataModel;

namespace PriconnePartyManager.Scripts.Common
{
    public class FileManager : Singleton<FileManager>
    {
        private const string FileNameUserParties = "./data/save/{0}.json";

        public T LoadJson<T>()
        {
            var fileName = string.Format(FileNameUserParties, typeof(T));
            if (!File.Exists(fileName))
            {
                return default;
            }
            var json = File.ReadAllText(fileName);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            var instance = JsonSerializer.Deserialize<T>(json, options);
            return instance;
        }

        public void SaveJson<T>(T data)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            var json = JsonSerializer.Serialize(data, options);
            var fileName = string.Format(FileNameUserParties, typeof(T));
            if (!File.Exists(fileName))
            {
                var dirName = Path.GetDirectoryName(fileName);
                if (!File.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                File.Create(fileName);
            }
            File.WriteAllText(fileName, json);
        }
    }
}