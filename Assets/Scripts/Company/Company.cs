using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Company
{
    [MoonSharpUserData]
    public class Company
    {
        public event EventHandler<Company> OnAssetsWorthCalculated;
        public event EventHandler<Company> OnDebtCalculated;

        public string tag;

        public Color color;
        public string name;

        [JsonIgnore]
        public bool isAI;

        public long cash;


        public long Networth => cash + assetsWorth - debt;

        public long assetsWorth;
        public long debt;

        public Company()
        {
            Ownership.OwnershipDictionary.OnOwnershipAdded += HandleOwnershipAdded;

            Loan.LoanDictionary.OnLoanAdded += HandleLoanAdded;
        }

        ~Company()
        {
            Ownership.OwnershipDictionary.OnOwnershipAdded -= HandleOwnershipAdded;

            Loan.LoanDictionary.OnLoanAdded -= HandleLoanAdded;
        }

        public long CalculateAssetsWorth()
        {
            long worth = 0;
            foreach (var type in Ownership.OwnershipDictionary.Dictionary[tag].Keys)
            {
                foreach (var id in Ownership.OwnershipDictionary.Dictionary[tag][type].Keys)
                {
                    Ownership.Ownership ownership = Ownership.OwnershipDictionary.Dictionary[tag][type][id];
                    Investment.Investment investment = Investment.InvestmentDictionary.GetInvestment(type, id);

                    worth += ownership.shares * investment.value;
                }
            }
            assetsWorth = worth;
            OnAssetsWorthCalculated?.Invoke(this, this);
            return worth;
        }

        public long CalculateDebt()
        {
            long debt = 0;
            foreach (var loan in Loan.LoanDictionary.Dictionary[tag])
            {
                debt += loan.amountLeft;
            }
            this.debt = debt;
            OnDebtCalculated?.Invoke(this, this);
            return debt;
        }

        private void HandleOwnershipAdded(object sender, Ownership.Ownership ownership)
        {
            if (ownership.company_tag != tag) return;

            CalculateAssetsWorth();
        }

        private void HandleOwnershipChanged(object sender, Ownership.Ownership ownership)
        {
            if (ownership.company_tag != tag) return;

            CalculateAssetsWorth();
        }

        private void HandleLoanAdded(object sender, Loan.Loan loan)
        {
            if (loan.company_tag != tag) return;

            CalculateDebt();
        }
    }
}
