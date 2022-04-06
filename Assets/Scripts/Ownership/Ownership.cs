using MoonSharp.Interpreter;

namespace Ownership
{
    [MoonSharpUserData]
    public class Ownership
    {
        public string company;
        public int investment;
        public Investment.Type type;
    }
}
