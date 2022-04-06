using MoonSharp.Interpreter;

namespace Map
{
    [MoonSharpUserData]
    public class OnMapModeChangedData
    {
        public int Mode { get; private set; }
        
        public OnMapModeChangedData(Mode mode)
        {
            Mode = (int)mode;
        }
    }
}

