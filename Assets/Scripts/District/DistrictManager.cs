using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace District
{
    [MoonSharpUserData]
    public class DistrictManager : Manager.Manager
    {
        public static DistrictManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, Data>> OnDistrictDataLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "district/manager.lua";

            LoadScript();
            LoadDistrictData();
            LoadDistrictData();

            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadDistrictContent(path);

            RaiseOnContentLoaded();
        }

        private void LoadDistrictData()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/district/districts.json");
            if (json == null) return;

            Data.LoadJson(json);
            OnDistrictDataLoaded?.Invoke(this, Data.Datas);
        }

        private void LoadDistrictContent(string path)
        {
            string json = Utils.ContentHandler.SafeGetString($"{path}/district/districts.json");
            if (json == null) return;

            DistrictDictionary.LoadJson(json);
        }
    }
}
