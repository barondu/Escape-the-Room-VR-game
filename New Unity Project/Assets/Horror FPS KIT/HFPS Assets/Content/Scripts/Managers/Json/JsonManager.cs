using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ThunderWire.JsonManager
{
    [Serializable]
    public enum FilePath
    {
        GameDataPath,
        GameSavesPath,
        DocumentsPath
    }

    /// <summary>
    /// Provides methods for writing and reading JSON files.
    /// </summary>
    public static class JsonManager
    {
        private static string folderPath;
        private static string fullPath;

        private static bool enableDebug = false;
        private static bool enableEncryption = false;
        private static bool pathSet = false;

        private static Dictionary<string, object> DimensionalArray = new Dictionary<string, object>();

        private static string jsonString = "";
        private static string cipherKey = "";

        public static void Settings(SaveLoadScriptable settings, bool debug = false)
        {
            cipherKey = settings.cipherKey;
            enableEncryption = settings.enableEncryption;
            folderPath = GetFilePath(settings.filePath);
            enableDebug = debug;
            pathSet = true;
        }

        private static string CheckFilename(string filename)
        {
            if (filename.Contains('.'))
            {
                return folderPath + filename;
            }
            else
            {
                return folderPath + filename + ".sav";
            }
        }

        public static string GetFilePath(FilePath Filepath)
        {
            if (Filepath == FilePath.GameSavesPath)
            {
                return Application.dataPath + "/Data/SavedGame/";
            }
            else if (Filepath == FilePath.GameDataPath)
            {
                return Application.dataPath + "/Data/";
            }
            else if (Filepath == FilePath.DocumentsPath)
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + Application.productName + "/" + "SavedGame/";
            }

            return null;
        }

        public static string GetCurrentPath()
        {
            return folderPath;
        }

        public static void ClearArray()
        {
            DimensionalArray.Clear();
            jsonString = "";
        }

        public static void UpdateArray(string Key, object Value)
        {
            DimensionalArray.Add(Key, Value);
        }

        public static string JsonOut()
        {
            return jsonString;
        }

        public static JObject Json()
        {
            JObject rss = JObject.Parse(jsonString);

            return rss;
        }

        public static JObject Json(string Json)
        {
            JObject rss = JObject.Parse(Json);

            return rss;
        }

        public static T Json<T>()
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static T JsonString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Function to Serialize Json Data by given Stream Asynchronously.
        /// </summary>
        public static async Task SerializeJsonDataAsync(Stream stream, bool isHidden = false)
        {
            string jsonString = JsonConvert.SerializeObject(DimensionalArray, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            using (StreamWriter sw = new StreamWriter(stream))
            {
                if (!enableEncryption)
                {
                    await sw.WriteAsync(jsonString);
                }
                else
                {
                    string es = EncryptData(jsonString);
                    await sw.WriteAsync(es);
                }
            }

            if (isHidden && File.Exists(fullPath))
            {
                new FileInfo(fullPath).Attributes = FileAttributes.Hidden;
            }

            if (enableDebug) { Debug.Log("<color=green>Game Saved: </color> " + ((FileStream)stream).Name); }
        }

        /// <summary>
        /// Function to Serialize Json Data
        /// </summary>
        public static void SerializeJsonData(string filename, bool isHidden = false)
        {
            string jsonString = JsonConvert.SerializeObject(DimensionalArray, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            if (!pathSet)
            {
                folderPath = GetFilePath(FilePath.GameSavesPath);
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (filename.Contains('.'))
            {
                fullPath = folderPath + filename;
            }
            else
            {
                fullPath = folderPath + filename + ".sav";
            }

            using (StreamWriter sw = new StreamWriter(fullPath))
            {
                if (!enableEncryption)
                {
                    sw.Write(jsonString);
                }
                else
                {
                    string es = EncryptData(jsonString);
                    sw.Write(es);
                }
            }

            if (isHidden && File.Exists(fullPath))
            {
                new FileInfo(fullPath).Attributes = FileAttributes.Hidden;
            }

            if (enableDebug) { Debug.Log("<color=green>Game Saved: </color> " + fullPath); }
        }

        /// <summary>
        /// Function to Serialize Json Data to FilePath
        /// </summary>
        public static void SerializeJsonData(FilePath filePath, string filename, bool isHidden = false)
        {
            string jsonString = JsonConvert.SerializeObject(DimensionalArray, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            folderPath = GetFilePath(filePath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (filename.Contains('.'))
            {
                fullPath = folderPath + filename;
            }
            else
            {
                fullPath = folderPath + filename + ".sav";
            }

            using (StreamWriter sw = new StreamWriter(fullPath))
            {
                if (!enableEncryption)
                {
                    sw.Write(jsonString);
                }
                else
                {
                    string es = EncryptData(jsonString);
                    sw.Write(es);
                }
            }

            if (isHidden && File.Exists(fullPath))
            {
                new FileInfo(fullPath).Attributes = FileAttributes.Hidden;
            }

            if (enableDebug) { Debug.Log("<color=green>Game Saved: </color> " + fullPath); }
        }

        /// <summary>
        /// Function to Deserialize Json String
        /// </summary>
        public static void DeserializeData(string filename)
        {
            if (filename.Contains('.'))
            {
                fullPath = folderPath + filename;
            }
            else
            {
                fullPath = folderPath + filename + ".sav";
            }

            if (!File.Exists(fullPath))
            {
                Debug.LogError("File (" + fullPath + ") does not exist!");
                return;
            }

            jsonString = DeserializeJsonData();
        }

        /// <summary>
        /// Function to Deserialize Json String from FilePath
        /// </summary>
        public static void DeserializeData(FilePath filePath, string filename)
        {
            if (filename.Contains('.'))
            {
                fullPath = GetFilePath(filePath) + filename;
            }
            else
            {
                fullPath = GetFilePath(filePath) + filename + ".sav";
            }

            if (!File.Exists(fullPath))
            {
                Debug.LogError("File (" + fullPath + ") does not exist!");
                return;
            }

            jsonString = DeserializeJsonData();
        }

        /// <summary>
        /// Function to Deserialize Json String by given Stream.
        /// </summary>
        public static void DeserializeData(Stream stream)
        {
            fullPath = ((FileStream)stream).Name;

            if (!File.Exists(fullPath))
            {
                Debug.LogError("File (" + fullPath + ") does not exist!");
                return;
            }

            jsonString = DeserializeJsonData();
        }

        private static string DeserializeJsonData()
        {
            string json = "";

            if (File.Exists(fullPath))
            {
                string jsonRead = File.ReadAllText(fullPath);

                if (enableEncryption)
                {
                    json = DecryptString(jsonRead);
                }
                else
                {
                    json = jsonRead;
                }

                if (enableDebug) { Debug.Log("<color=green>Json string readed successfully</color>"); }

                return json;
            }
            else
            {
                if (enableDebug) { Debug.Log("<color=red>File does not exist: </color> " + fullPath); }
            }

            return null;
        }

        private static string EncryptData(string toEncrypt)
        {
            byte[] result;
            byte[] IV;
            byte[] AESkey = Encoding.UTF8.GetBytes(cipherKey);

            using (Aes aes = Aes.Create())
            {
                aes.Key = AESkey;
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;

                IV = aes.IV;
                byte[] DataToEncrypt = Encoding.UTF8.GetBytes(toEncrypt);

                try
                {
                    ICryptoTransform Encryptor = aes.CreateEncryptor();
                    result = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                }
                finally
                {
                    aes.Clear();
                }
            }

            byte[] cmbIV = new byte[IV.Length + result.Length];
            Array.Copy(IV, 0, cmbIV, 0, IV.Length);
            Array.Copy(result, 0, cmbIV, IV.Length, result.Length);

            return Convert.ToBase64String(cmbIV);
        }

        private static string DecryptString(string toDecrypt)
        {
            byte[] result;
            byte[] DataToDecrypt = Convert.FromBase64String(toDecrypt);
            byte[] AESkey = Encoding.UTF8.GetBytes(cipherKey);

            using (Aes aes = Aes.Create())
            {
                aes.Key = AESkey;

                byte[] IV = new byte[aes.BlockSize / 8];
                byte[] cipherText = new byte[DataToDecrypt.Length - IV.Length];
                Array.Copy(DataToDecrypt, IV, IV.Length);
                Array.Copy(DataToDecrypt, IV.Length, cipherText, 0, cipherText.Length);

                aes.IV = IV;
                aes.Mode = CipherMode.CBC;

                try
                {
                    ICryptoTransform Encryptor = aes.CreateDecryptor();
                    result = Encryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                }
                finally
                {
                    aes.Clear();
                }
            }

            return Encoding.UTF8.GetString(result);
        }
    }
}