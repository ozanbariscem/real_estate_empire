using System;
using MoonSharp.Interpreter;

namespace Loan
{
    [Serializable]
    [MoonSharpUserData]
    public class Type
    {
        public int id;
        public string type; // "networth_percentage", "fixed_amount"
        public string name;
        public uint amount;
        public byte interest;
        public byte payment_length;
    }
}

