using MoonSharp.Interpreter;

namespace Load
{
    [MoonSharpUserData]
    public class Progress
    {
        public string message;
        public float delta;

        public Progress(string msg, float delta) { message = msg; this.delta = delta; }
    }
}

