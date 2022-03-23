using System.Collections.Generic;
using MoonSharp.Interpreter;
using System;

namespace Loan
{
    [MoonSharpUserData]
    public class LoanDictionary
    {
        public static event EventHandler<Loan> OnLoanAdded;

        public static Dictionary<string, List<Loan>> Dictionary { get; private set; }

        public static void LoadLoans(List<Loan> loans)
        {
            if (Dictionary == null)
                Dictionary = new Dictionary<string, List<Loan>>();

            foreach (var loan in loans)
            {
                if (!Dictionary.TryGetValue(loan.company_tag, out var _loans))
                {
                    Dictionary.Add(loan.company_tag, new List<Loan>() { loan });
                    OnLoanAdded?.Invoke(null, loan);
                }
            }
        }

        public static List<Loan> ToList()
        {
            List<Loan> loans = new List<Loan>();

            foreach (var key in Dictionary.Keys)
            {
                loans.AddRange(Dictionary[key]);
            }

            return loans;
        }
    }
}

