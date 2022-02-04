using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Investor
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;
        public event Action<List<Investor>> OnInvestorsLoaded;

        private Script script;

        private void Start()
        {
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        [MoonSharpHidden]
        public void Initialize()
        {
            LoadScript();
            LoadInvestors();
        }

        #region SUBSCRIBE
        private void Subscribe()
        {
        }

        private void Unsubscribe()
        {
        }
        #endregion

        #region CONTENT LOADER
        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/investor/manager.lua");
            if (scriptString == null) return;

            UserData.RegisterType<Investor>();
            UserData.RegisterType<InvestorList>();
            UserData.RegisterType<Console.Console>();

            script = new Script();
            script.Globals["GetInvestor"] = (Func<int, Investor>)InvestorList.GetInvestor;
            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke();
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }

        private void LoadInvestors()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/investor/investors.json");
            if (json == null) return;

            List<Investor> investors = JsonConvert.DeserializeObject<List<Investor>>(json);

            InvestorList list = new InvestorList(investors);

            OnInvestorsLoaded?.Invoke(investors);
            script.Call(script.Globals[nameof(OnInvestorsLoaded)], investors);
        }
        #endregion
    }
}
