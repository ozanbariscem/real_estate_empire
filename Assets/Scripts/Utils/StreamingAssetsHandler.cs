using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Utils
{
    [MoonSharpUserData]
    public static class StreamingAssetsHandler
    {
        public static Script SafeGetScript(string target)
        {
            return ContentHandler.SafeGetScript(Path.Combine(Application.streamingAssetsPath, target));
        }

        public static Texture2D SafeGetTexture(string target)
        {
            return ContentHandler.SafeGetTexture(Path.Combine(Application.streamingAssetsPath, target));
        }

        public static string SafeGetString(string target)
        {
            return ContentHandler.SafeGetString(Path.Combine(Application.streamingAssetsPath, target));
        }

        public static void SafeSetString(string path, string content)
        {
            ContentHandler.SafeSetString(Path.Combine(Application.streamingAssetsPath, path), content);
        }
    }
}

