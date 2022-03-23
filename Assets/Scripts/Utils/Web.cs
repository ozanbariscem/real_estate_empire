using System.Diagnostics;

namespace Utils
{
    public static class Web 
    {
        public static void Youtube() => Open("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        public static void Twitter() => Open("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        public static void Discord() => Open("https://www.youtube.com/watch?v=dQw4w9WgXcQ");

        private static void Open(string url)
        {
            Process.Start(url);
        }
    }
}

