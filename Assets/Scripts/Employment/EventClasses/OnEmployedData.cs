using Person.Employee;
using MoonSharp.Interpreter;

namespace Employment
{
    [MoonSharpUserData]
    public class OnEmployedData
    {
        public string company;
        public int employee;
        public bool hired;

        public OnEmployedData(string c, int e, bool h)
        {
            company = c;
            employee = e;
            hired = h;
        }
    }
}
