using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public static class StringConversions
{
    public static string ToCash(long cash)
    {
        if (cash < 0)
            return $"-" + ToCash(Math.Abs(cash));

        return cash.ToString("C0");
    }

    public static string ToShortCash(long n)
    {
        if (n < 0)
            return "-" + ToShortCash(Math.Abs(n));

        if (n < 1000)
            return "$" + n.ToString();
        if (n < 10000)
            return "$" + string.Format("{0:#,.##}K", n - 5);
        if (n < 100000)
            return "$" + string.Format("{0:#,.#}K", n - 50);
        if (n < 1000000)
            return "$" + string.Format("{0:#,.}K", n - 500);
        if (n < 10000000)
            return "$" + string.Format("{0:#,,.##}M", n - 5000);
        if (n < 100000000)
            return "$" + string.Format("{0:#,,.#}M", n - 50000);
        if (n < 1000000000)
            return "$" + string.Format("{0:#,,.}M", n - 500000);
        return "$" + string.Format("{0:#,,,.##}B", n - 5000000);
    }
}
