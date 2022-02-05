using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Investor
{
    [MoonSharpUserData]
    public class InvestorManager : Manager.Manager
    {
        public static InvestorManager Instance { get; private set; }

        public event EventHandler<List<Investor>> OnInvestorsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "investor/manager.lua";
            LoadScript();
            LoadInvestors();

            RaiseOnRulesLoaded();
        }

        #region CONTENT LOADER
        private void LoadInvestors()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/investor/investors.json");
            if (json == null) return;

            List<Investor> investors = JsonConvert.DeserializeObject<List<Investor>>(json);
            InvestorList list = new InvestorList(investors);

            OnInvestorsLoaded?.Invoke(this, investors);
        }
        #endregion
    }
}
