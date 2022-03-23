using System;
using System.Linq;
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

        #region MISC
        [ContextMenu("PopulateDistricts")]
        public void PopulateDistricts()
        {
            int id = 0;
            foreach (District district in DistrictDictionary.Dictionary.Values)
            {
                district.properties = new List<int>();

                for (int i = 0; i < district.Size; i++)
                {
                    Investment.Investment invesment = Investment.InvestmentDictionary.GetInvestment("property", id);
                    if (invesment != null)
                    {
                        district.properties.Add(invesment.id);
                        id++;
                    }
                    else break;
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(DistrictDictionary.Dictionary.Values.OrderBy(x => x.tag).ToList(), Newtonsoft.Json.Formatting.Indented);
            Utils.StreamingAssetsHandler.SafeSetString($"vanilla/scenarios/New Game/district/districts.json", json);
        }
        #endregion
    }
}
