using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Invesment
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;
        public event Action<Dictionary<string, Type>> OnTypesLoaded;
        public event Action<Dictionary<string, List<Invesment>>> OnInvesmentsLoaded;
        public event Action OnReady;

        private Dictionary<string, List<Invesment>> invesments;

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
            LoadTypes();
            LoadInvesments();

            OnReady?.Invoke();
            script.Call(script.Globals[nameof(OnReady)]);
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
        private void Subscribe()
        {
            Time.Manager.Instance.OnDayPass += HandleDayPass;
            Time.Manager.Instance.OnMonthPass += HandleMonthPass;
            Time.Manager.Instance.OnYearPass += HandleYearPass;
        }

        private void Unsubscribe()
        {
            Time.Manager.Instance.OnDayPass -= HandleDayPass;
            Time.Manager.Instance.OnMonthPass -= HandleMonthPass;
            Time.Manager.Instance.OnYearPass -= HandleYearPass;
        }
        #endregion

        #region CONTENT LOADER
        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/invesment/manager.lua");
            if (scriptString == null) return;

            UserData.RegisterType<Data>();
            UserData.RegisterType<Type>();
            UserData.RegisterType<Invesment>();

            UserData.RegisterType<Console.Console>();

            script = new Script();
            script.Globals["GetInvesment"] = (Func<string, int, Invesment>)InvesmentDictionary.GetInvesment;
            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke();
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }

        private void LoadTypes()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/invesment/types/types.json");
            if (json == null) return;

            Types.LoadJson(json);

            OnTypesLoaded?.Invoke(Types.Dictionary);
            script.Call(script.Globals[nameof(OnTypesLoaded)], Types.Dictionary);
        }

        private void LoadInvesments()
        {
            invesments = new Dictionary<string, List<Invesment>>();
            foreach (Type type in Types.Dictionary.Values)
            {
                string json = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/invesment/{type.type}/invesments.json");
                if (json == null) continue;

                if (!invesments.ContainsKey(type.type))
                    invesments.Add(type.type, new List<Invesment>());

                List<Invesment> _invesments = JsonConvert.DeserializeObject<List<Invesment>>(json);
                invesments[type.type].AddRange(_invesments);
            }

            InvesmentDictionary invesmentList = new InvesmentDictionary(invesments);

            OnInvesmentsLoaded?.Invoke(invesments);
            script.Call(script.Globals[nameof(OnInvesmentsLoaded)], invesments);
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
