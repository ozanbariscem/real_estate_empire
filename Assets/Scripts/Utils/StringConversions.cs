using MoonSharp.Interpreter;

[MoonSharpUserData]
public static class StringConversions
{
    public static string ToCash(long cash)
    {
        return cash.ToString("C0");
    }
}
