using System;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Invesment
{
    [MoonSharpUserData]
    public class InvesmentManager : Manager.Manager
    {
        public static InvesmentManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, Type>> OnTypesLoaded;
        public event EventHandler<Dictionary<string, List<Invesment>>> OnInvesmentsLoaded;

        private Dictionary<string, List<Invesment>> invesments;

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
            foreach (var type in invesments.Keys)
            {
                Script script = Types.Dictionary[type].script;
                if (script.Globals[eventName] != null)
                {
                    script.Call(script.Globals[eventName], date, invesments[type]);
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

        private void LoadInvesments(string path)
        {
            invesments = new Dictionary<string, List<Invesment>>();
            foreach (Type type in Types.Dictionary.Values)
            {
                string json = Utils.ContentHandler.SafeGetString($"{path}/invesment/{type.type}/invesments.json");
                if (json == null) continue;

                if (!invesments.ContainsKey(type.type))
                    invesments.Add(type.type, new List<Invesment>());

                List<Invesment> _invesments = JsonConvert.DeserializeObject<List<Invesment>>(json);
                invesments[type.type].AddRange(_invesments);
            }

            InvesmentDictionary invesmentList = new InvesmentDictionary(invesments);
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
