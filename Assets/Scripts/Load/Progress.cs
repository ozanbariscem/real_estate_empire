using MoonSharp.Interpreter;

namespace Load
{
    [MoonSharpUserData]
    public class Progress
    {
        public string message;

        public Progress(string msg) { message = msg; }
    }
}

