using System;
using System.IO;
using MoonSharp.Interpreter;

namespace SaveFile
{
    [MoonSharpUserData]
    public class Data
    {
        public string path;
        public string name;
        public string date;

        [MoonSharpHidden]
        public DateTime lastWriteTime;

        private string status;
        public string Status => status;

        public static Data FromFileInfo(FileInfo fileInfo, string status)
        {
            return new Data()
            {
                path = fileInfo.FullName,
                name = Path.GetFileNameWithoutExtension(fileInfo.Name),
                lastWriteTime = fileInfo.LastWriteTime,
                date = fileInfo.LastWriteTime.ToString(),
                status = status
            };
        }
    }
}

