using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Company
{
    [MoonSharpUserData]
    public class Company
    {
        public static event EventHandler<Company> OnEmployeeLimitChanged;
        public static event EventHandler<Company> OnPropertyLimitChanged;

        public static event EventHandler<Company> OnSalaryPaymentsCalculated;
        public static event EventHandler<Company> OnLoanPaymentsCalculated;
        public static event EventHandler<Company> OnExpensesCalculated;

        public static event EventHandler<Company> OnAssetsWorthCalculated;
        public static event EventHandler<Company> OnDebtCalculated;
        
        public static event EventHandler<Company> OnCashChanged;

        public string tag;

        public Color color;
        public string name;

        [JsonIgnore] public bool isAI;

        [JsonIgnore] private long cash;
        public long Cash
        {
            get { return cash; }
            set
            {
                cash = value;
                OnCashChanged?.Invoke(this, this);
            }
        }

        [JsonIgnore] public long Networth => cash + assetsWorth - debt;
        [JsonIgnore] public long assetsWorth;
        [JsonIgnore] public long debt;

        // ----- FINANCE -----
        [JsonIgnore] public long salaryPayments;
        [JsonIgnore] public long loanPayments;

        [JsonIgnore] public long MonthlyNet => 0 - Expenses;
        [JsonIgnore] public long Expenses => salaryPayments + loanPayments;


        // ----- LIMITS -----
        [JsonIgnore] private long employee_limit;
        [JsonIgnore] public long EmployeeLimit
        {
            get { return employee_limit; }
            set
            {
                employee_limit = value;
                OnEmployeeLimitChanged?.Invoke(this, this);
            }
        }

        [JsonIgnore] private long property_limit;
        [JsonIgnore] public long PropertyLimit
        {
            get => property_limit;
            set
            {
                property_limit = value;
                OnPropertyLimitChanged?.Invoke(this, this);
            }
        }


        // ---- COUNTS -----
        [JsonIgnore] public int EmployeeCount { get; private set; }
        [JsonIgnore] public int PropertyCount { get; private set; }

        public Company()
        {
            EmployeeLimit = Constants.Values.base_employee_limit;
            PropertyLimit = Constants.Values.base_property_limit;

            Ownership.OwnershipDictionary.OnOwnershipAdded += HandleOwnershipAdded;
            Loan.LoanDictionary.OnLoanAdded += HandleLoanAdded;
            Employment.EmploymentManager.Instance.OnEmployed += HandleEmployed;
            Time.TimeManager.Instance.OnMonthPass += HandleMonthPass;
        }

        ~Company()
        {
            Ownership.OwnershipDictionary.OnOwnershipAdded -= HandleOwnershipAdded;
            Loan.LoanDictionary.OnLoanAdded -= HandleLoanAdded;
            Employment.EmploymentManager.Instance.OnEmployed -= HandleEmployed;
            Time.TimeManager.Instance.OnMonthPass -= HandleMonthPass;
        }

        public long CalculateAssetsWorth()
        {
            long worth = 0;
            
            if (Ownership.OwnershipDictionary.CompanyCategorized.TryGetValue(tag, out var dict))
            {
                foreach (var value in dict.Values)
                {
                    foreach (var investment in value.Keys)
                    {
                        worth += Investment.Property.ApartmentDictionary.Apartments[investment].price;
                    }
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

        #region MONTHLY_PAYMENTS
        public long CalculateLoanPayments()
        {
            var loans = Loan.LoanDictionary.SafeGet(tag);

            long total = 0;
            foreach (var loan in loans)
            {
                total += loan.Type.amount / loan.paymentLeft;
            }
            loanPayments = total;
            OnLoanPaymentsCalculated?.Invoke(this, this);
            OnExpensesCalculated?.Invoke(this, this);
            return total;
        }

        public long CalculateSalaryPayments()
        {
            var employees = Employment.EmploymentDictionary.SafeGetEmployeeIdsOfCompany(tag);

            long total = 0;
            foreach (var employee_id in employees)
            {
                var employee = Person.Employee.EmployeeDictionary.Employees[employee_id];
                total += (long)employee.Salary;
            }
            salaryPayments = total/12;
            OnSalaryPaymentsCalculated?.Invoke(this, this);
            OnExpensesCalculated?.Invoke(this, this);
            return total;
        }
        #endregion

        private void HandleMonthPass(object sender, Time.Date date)
        {
            Cash = Cash + MonthlyNet;
        }

        private void HandleOwnershipAdded(object sender, Ownership.Ownership ownership)
        {
            if (ownership.company != tag) return;
            CalculateAssetsWorth();
            PropertyCount++;
        }

        private void HandleOwnershipChanged(object sender, Ownership.Ownership ownership)
        {
            if (ownership.company != tag) return;

            CalculateAssetsWorth();
        }

        private void HandleLoanAdded(object sender, Loan.Loan loan)
        {
            if (loan.company_tag != tag) return;

            CalculateLoanPayments();
            CalculateDebt();
        }
    
        private void HandleEmployed(object sender, Employment.OnEmployedData data)
        {
            if (data.company == tag)
            {
                CalculateSalaryPayments();
            }
            if (data.hired) EmployeeCount++;
            else EmployeeCount--;
        }
    }
}
