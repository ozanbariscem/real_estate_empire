using MoonSharp.Interpreter;

namespace Company
{
    [MoonSharpUserData]
    public class Constants
    {
        public static Constants Values;

        public int base_employee_limit;
        public int base_property_limit;

        public Constants()
        {
            Values = this;
        }
    }
}
