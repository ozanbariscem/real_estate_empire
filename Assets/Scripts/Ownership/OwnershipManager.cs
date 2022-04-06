using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Newtonsoft.Json;

namespace Ownership
{
    [MoonSharpUserData]
    public class OwnershipManager : Manager.Manager
    {
        public static OwnershipManager Instance { get; private set; }

        public event EventHandler<List<Ownership>> OnOwnershipsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "ownership/manager.lua";
            LoadScript();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadOwnerships(path);
            RaiseOnContentLoaded();
        }

        #region CONTENT LOADER
        private void LoadOwnerships(string path)
        {
            string json = Utils.ContentHandler.SafeGetString($"{path}/ownership/ownerships.json");
            if (json == null) return;

            List<Ownership> ownerships = JsonConvert.DeserializeObject<List<Ownership>>(json);

            OwnershipDictionary.LoadOwnerships(ownerships);
            OnOwnershipsLoaded?.Invoke(this, ownerships);
        }
        #endregion
    }
}
