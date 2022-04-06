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

        public static List<Loan> SafeGet(string tag)
        {
            if (Dictionary != null && Dictionary.TryGetValue(tag, out var loans))
                return loans;
            return new List<Loan>();
        }

        public static void AddLoan(Loan loan)
        {
            if (Dictionary == null) 
                Dictionary = new Dictionary<string, List<Loan>>();

            if (Dictionary.TryGetValue(loan.company_tag, out var _loans))
                _loans.Add(loan);
            else
                Dictionary.Add(loan.company_tag, new List<Loan>() { loan });
            OnLoanAdded?.Invoke(null, loan);
        }

        public static void LoadLoans(List<Loan> loans)
        {
            foreach (var loan in loans)
            {
                AddLoan(loan);
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

