using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using UnityEngine;

namespace Loan
{
    [MoonSharpUserData]
    public class LoanManager : Manager.Manager
    {
        public static LoanManager Instance { get; private set; }

        public event EventHandler<Dictionary<int, Type>> OnTypesLoaded;
        public event EventHandler<Dictionary<int, Loan>> OnLoansLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "loan/manager.lua";
            LoadScript();
            LoadTypes();

            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadLoans(path);
            RaiseOnContentLoaded();
        }

        private void LoadTypes()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/loan/types.json");
            if (json == null) return;

            List<Type> types = JsonConvert.DeserializeObject<List<Type>>(json);

            Types.SetTypes(types);
            OnTypesLoaded?.Invoke(this, Types.Dictionary);
        }

        private void LoadLoans(string path)
        {
            string json = Utils.ContentHandler.SafeGetString($"{path}/loan/loans.json");
            if (json == null) return;

            List<Loan> loans = JsonConvert.DeserializeObject<List<Loan>>(json);
            LoanList.Set(loans);
            OnLoansLoaded?.Invoke(this, LoanList.Loans);
        }
    }
}

