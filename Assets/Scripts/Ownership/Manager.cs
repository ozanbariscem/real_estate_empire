using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using Newtonsoft.Json;

namespace Ownership
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;
        public event Action<List<Ownership>> OnOwnershipsLoaded;

        private Script script;

        [MoonSharpHidden]
        public void Initialize()
        {
            LoadScript();
            LoadInvestors();
        }

        #region CONTENT LOADER
        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/ownership/manager.lua");
            if (scriptString == null) return;

            UserData.RegisterType<Ownership>();
            UserData.RegisterType<OwnershipList>();
            UserData.RegisterType<Console.Console>();

            script = new Script();
            script.Globals["GetOwnership"] = (Func<int, Ownership>)OwnershipList.GetOwnership;
            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke();
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }

        private void LoadInvestors()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/ownership/ownerships.json");
            if (json == null) return;

            List<Ownership> ownerships = JsonConvert.DeserializeObject<List<Ownership>>(json);

            OwnershipList list = new OwnershipList(ownerships);

            OnOwnershipsLoaded?.Invoke(ownerships);
            script.Call(script.Globals[nameof(OnOwnershipsLoaded)], ownerships);
        }
        #endregion
    }
}
