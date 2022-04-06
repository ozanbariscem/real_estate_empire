using System;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Person
{
    [MoonSharpUserData]
    public class PersonManager : Manager.Manager
    {
        public static PersonManager Instance { get; private set; }

        public event EventHandler<Constants> OnConstantsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "person/manager.lua";

            LoadScript();
            LoadConstants();
            RaiseOnRulesLoaded();
        }

        private void LoadConstants()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/person/constants.json");
            if (json == null) return;

            Constants constants = JsonConvert.DeserializeObject<Constants>(json);
            OnConstantsLoaded?.Invoke(this, constants);
        }
    }
}
