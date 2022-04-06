using System;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Investment.Property
{
    [MoonSharpUserData]
    public class BuildingManager : Manager.Manager
    {
        public static BuildingManager Instance { get; private set; }

        public static event EventHandler<Dictionary<string, Dictionary<int, Building>>> OnBuildingsLoaded;
        public static event EventHandler<Dictionary<int, Apartment>> OnApartmentsLoaded;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "investment/property/manager.lua";

            LoadScript();
            LoadBuildings();
            LoadApartments();
            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            RaiseOnContentLoaded();
        }

        private void LoadBuildings()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/investment/property/buildings.json");
            if (json == null) return;

            BuildingDictionary.LoadBuildings(JsonConvert.DeserializeObject<List<Building>>(json));
            OnBuildingsLoaded?.Invoke(this, BuildingDictionary.Buildings);
        }

        private void LoadApartments()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/investment/property/apartments.json");
            if (json == null) return;

            ApartmentDictionary.LoadApartments(JsonConvert.DeserializeObject<List<Apartment>>(json));
            OnApartmentsLoaded?.Invoke(this, ApartmentDictionary.Apartments);
        }
    }
}
