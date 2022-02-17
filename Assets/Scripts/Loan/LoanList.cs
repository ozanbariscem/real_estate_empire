using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Loan
{
    [MoonSharpUserData]
    public class LoanList
    {
        private static Dictionary<int, Loan> loans;
        public static Dictionary<int, Loan> Loans => loans;

        public static void Set(List<Loan> _loans)
        {
            loans = new Dictionary<int, Loan>();
            foreach (var loan in _loans)
            {
                if (loans.ContainsKey(loan.id)) continue; // Can't override a loan on a save file it doesnt make sense so we continue
                else loans.Add(loan.id, loan);
            }
        }
    }
}

