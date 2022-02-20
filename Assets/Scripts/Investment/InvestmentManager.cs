using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using UnityEngine;

namespace Investment
{
    [MoonSharpUserData]
    public class InvestmentManager : Manager.Manager
    {
        public static InvestmentManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, Type>> OnTypesLoaded;
        public event EventHandler<Dictionary<string, Dictionary<int, Investment>>> OnInvestmentsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "investment/manager.lua";

            LoadScript();
            LoadTypes();
            LoadPhotos();

            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadInvestments(path);
            RaiseOnContentLoaded();
        }

        #region CONTENT LOADER
        private void LoadTypes()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/investment/types/types.json");
            if (json == null) return;
            
            Types.LoadJson(json);
            OnTypesLoaded?.Invoke(this, Types.Dictionary);
        }

        private void LoadPhotos()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "vanilla/investment/photos");

            string[] files = Directory.GetFiles(path);

            Dictionary<string, Texture2D> photos = new Dictionary<string, Texture2D>();
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                if (info.Extension != ".png" && info.Extension != ".jpeg" && info.Extension != ".jpg") continue;

                Texture2D texture = Utils.ContentHandler.SafeGetTexture($"{file}");
                photos.Add(Path.GetFileNameWithoutExtension(info.Name), texture);
            }
            InvestmentDictionary.SetPhotos(photos);
        }

        private void LoadInvestments(string path)
        {
            Dictionary<string, Dictionary<int, Investment>>  invesments = new Dictionary<string, Dictionary<int, Investment>>();
            foreach (Type type in Types.Dictionary.Values)
            {
                string json = Utils.ContentHandler.SafeGetString($"{path}/investment/{type.type}/investments.json");
                if (json == null) continue;

                if (!invesments.ContainsKey(type.type))
                    invesments.Add(type.type, new Dictionary<int, Investment>());

                List<Investment> _invesments = JsonConvert.DeserializeObject<List<Investment>>(json);
                foreach (var invesment in _invesments)
                {
                    invesment.texture = InvestmentDictionary.Photos[invesment.photo];
                }
                invesments[type.type] = InvestmentDictionary.ConvertListOfInvestmentsToDictioanry(_invesments);
            }

            InvestmentDictionary.SetInvestments(invesments);
            OnInvestmentsLoaded?.Invoke(this, invesments);
        }
        #endregion

        #region MISC
#if UNITY_EDITOR
        [ContextMenu("Create Random Properties")]
        public void CreateRandomProperties()
        {
            List<Investment> invesments = new List<Investment>();

            int id = 0;
            foreach (District.District district in District.DistrictDictionary.Dictionary.Values)
            {
                for (int i = 0; i < district.Size; i++)
                {
                    Investment invesment = new Investment();
                    invesment.id = id++;
                    invesment.type = "property";

                    int rnd = UnityEngine.Random.Range(0, Types.Dictionary["property"].subTypes.Count);
                    invesment.sub_type = Types.Dictionary["property"].subTypes.Keys.ToArray()[rnd];
                    invesment.photo = $"{invesment.sub_type}_generic";

                    invesment.name = $"Very Good {invesment.sub_type}";

                    invesment.age = -1;
                    if (invesment.Data.Is("ageable"))
                        invesment.age = (short)UnityEngine.Random.Range(0, 121);

                    invesment.shares = 1;
                    if (invesment.Data.Is("partially_ownable"))
                        invesment.shares = (ushort)UnityEngine.Random.Range(10, 500);

                    invesment.baseValue = 10_000_000;
                    if (invesment.Data.Is("partially_ownable"))
                        invesment.baseValue = (uint)UnityEngine.Random.Range(100_000, 10_000_000);

                    invesments.Add(invesment);
                }
            }

            string json = JsonConvert.SerializeObject(invesments, Formatting.Indented);
            Utils.StreamingAssetsHandler.SafeSetString($"vanilla/scenarios/New Game/investment/property/investments.json", json);
        }

        // To easily id every property safely and properly
        public void IDProperties()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/investment/property/investments.json");
            if (json == null) return;

            List<Investment> _invesments = JsonConvert.DeserializeObject<List<Investment>>(json);
            int i = 0;
            foreach (Investment invesment in _invesments)
            {
                invesment.id = i;
                i++;
            }

            json = JsonConvert.SerializeObject(_invesments, Formatting.Indented);
            Utils.StreamingAssetsHandler.SafeSetString($"vanilla/investment/property/investments.json", json);
        }
#endif
        #endregion
    }
}
