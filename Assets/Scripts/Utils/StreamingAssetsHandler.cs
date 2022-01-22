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
    }
}

