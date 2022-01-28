using System.IO;
using UnityEngine;

namespace Utils
{
    public static class StreamingAssetsHandler
    {
        public static string SafeGetString(string target)
        {
            string path = Path.Combine(Application.streamingAssetsPath, target);

            if (!File.Exists(path))
            {
                Debug.LogError($"Hey make sure {path} exists!");
                return null;
            }

            StreamReader sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            sr.Close();

            return json;
        }

        public static void SafeSetString(string path, string content)
        {
            string file = Path.Combine(Application.streamingAssetsPath, path);

            if (!File.Exists(file))
            {
                Debug.LogError($"Hey make sure {file} exists!");
                return;
            }

            StreamWriter sw = new StreamWriter(file);
            sw.Write(content);
            sw.Close();
        }
    }
}

