using System;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Utils;

namespace PriconnePartyManager.Scripts.Common
{
    public class FileManager : Singleton<FileManager>
    {
        private const string FileNameUserParties = "./data/save/{0}.json";

        public T LoadJsonFromFile<T>(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = string.Format(FileNameUserParties, typeof(T).Name);
            }
            if (!File.Exists(path))
            {
                return default;
            }
            var json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return LoadJson<T>(json);
        }

        public T LoadJson<T>(string json)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };

            var instance = JsonSerializer.Deserialize<T>(json, options);
            return instance;
        }

        public void SaveJson<T>(T data, string path = null)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            var json = JsonSerializer.Serialize(data, options);
            if (string.IsNullOrEmpty(path))
            {
                path = string.Format(FileNameUserParties, typeof(T).Name);
            }
            if (!File.Exists(path))
            {
                var dirName = Path.GetDirectoryName(path);
                if (!File.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                File.Create(path);
            }
            File.WriteAllText(path, json);
        }
        
        
        /// <summary>
        /// Brotli形式の解凍
        /// </summary>
        public bool DecompressBrotli(string preDecompressFilePath, string decompressedFilePath, bool deleteFromFile = false)
        {
            try
            {
                using var preDecompressFile = new FileStream(
                    preDecompressFilePath,
                    FileMode.Open,
                    FileAccess.Read
                );

                using var decompressedFile = new FileStream(
                    decompressedFilePath,
                    FileMode.Create,
                    FileAccess.Write
                );

                using var bs = new BrotliStream(preDecompressFile, CompressionMode.Decompress);
                bs.CopyTo(decompressedFile);
                
                preDecompressFile.Close();
                decompressedFile.Close();
                bs.Close();

                if (deleteFromFile)
                {
                    File.Delete(preDecompressFilePath);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}