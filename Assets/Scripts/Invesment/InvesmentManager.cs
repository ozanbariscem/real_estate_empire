using System;
using System.IO;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using UnityEngine;

namespace Invesment
{
    [MoonSharpUserData]
    public class InvesmentManager : Manager.Manager
    {
        public static InvesmentManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, Type>> OnTypesLoaded;
        public event EventHandler<Dictionary<string, Dictionary<int, Invesment>>> OnInvesmentsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "invesment/manager.lua";

            LoadScript();
            LoadTypes();
            LoadPhotos();

            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadInvesments(path);
            RaiseOnContentLoaded();
        }

        #region HANDLERS
        private void HandleHourPass(object sender, Time.Date date)
        {
            HandleTimePass("HandleHourPass", date);
        }

        private void HandleDayPass(object sender, Time.Date date)
        {
            HandleTimePass("HandleDayPass", date);
        }

        private void HandleMonthPass(object sender, Time.Date date)
        {
            HandleTimePass("HandleMonthPass", date);
        }

        private void HandleYearPass(object sender, Time.Date date)
        {
            HandleTimePass("HandleYearPass", date);
        }

        private void HandleTimePass(string eventName, Time.Date date)
        {
            foreach (var type in InvesmentDictionary.Invesments.Keys)
            {
                Script script = Types.Dictionary[type].script;
                if (script.Globals[eventName] != null)
                {
                    script.Call(script.Globals[eventName], date, InvesmentDictionary.Invesments[type]);
                }
            }
        }
        #endregion

        #region SUBSCRIPTIONS
        protected override void Subscribe()
        {
            Time.TimeManager.Instance.OnHourPass += HandleHourPass;
            Time.TimeManager.Instance.OnDayPass += HandleDayPass;
            Time.TimeManager.Instance.OnMonthPass += HandleMonthPass;
            Time.TimeManager.Instance.OnYearPass += HandleYearPass;
        }

        protected override void Unsubscribe()
        {
            Time.TimeManager.Instance.OnHourPass -= HandleHourPass;
            Time.TimeManager.Instance.OnDayPass -= HandleDayPass;
            Time.TimeManager.Instance.OnMonthPass -= HandleMonthPass;
            Time.TimeManager.Instance.OnYearPass -= HandleYearPass;
        }
        #endregion

        #region CONTENT LOADER
        private void LoadTypes()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/invesment/types/types.json");
            if (json == null) return;
            
            Types.LoadJson(json);
            OnTypesLoaded?.Invoke(this, Types.Dictionary);
        }

        private void LoadPhotos()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "vanilla/invesment/photos");

            string[] files = Directory.GetFiles(path);

            Dictionary<string, Texture2D> photos = new Dictionary<string, Texture2D>();
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                if (info.Extension != ".png" && info.Extension != ".jpeg" && info.Extension != ".jpg") continue;

                Texture2D texture = Utils.ContentHandler.SafeGetTexture($"{file}");
                photos.Add(Path.GetFileNameWithoutExtension(info.Name), texture);
            }
            InvesmentDictionary.SetPhotos(photos);
        }

        private void LoadInvesments(string path)
        {
            Dictionary<string, Dictionary<int, Invesment>>  invesments = new Dictionary<string, Dictionary<int, Invesment>>();
            foreach (Type type in Types.Dictionary.Values)
            {
                string json = Utils.ContentHandler.SafeGetString($"{path}/invesment/{type.type}/invesments.json");
                if (json == null) continue;

                if (!invesments.ContainsKey(type.type))
                    invesments.Add(type.type, new Dictionary<int, Invesment>());

                List<Invesment> _invesments = JsonConvert.DeserializeObject<List<Invesment>>(json);
                foreach (var invesment in _invesments)
                {
                    invesment.texture = InvesmentDictionary.Photos[invesment.photo];
                }
                invesments[type.type] = InvesmentDictionary.ConvertListOfInvesmentsToDictioanry(_invesments);
            }

            InvesmentDictionary.SetInvesments(invesments);
            OnInvesmentsLoaded?.Invoke(this, invesments);
        }
        #endregion

        #region MISC
#if UNITY_EDITOR
        // To easily id every property safely and properly
        public void IDProperties()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/invesment/property/invesments.json");
            if (json == null) return;

            List<Invesment> _invesments = JsonConvert.DeserializeObject<List<Invesment>>(json);
            int i = 0;
            foreach (Invesment invesment in _invesments)
            {
                invesment.id = i;
                i++;
            }

            json = JsonConvert.SerializeObject(_invesments, Formatting.Indented);
            Utils.StreamingAssetsHandler.SafeSetString($"vanilla/invesment/property/invesments.json", json);
        }
#endif
        #endregion
    }
}
