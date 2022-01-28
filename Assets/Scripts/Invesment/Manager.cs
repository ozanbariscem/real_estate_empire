using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Invesment
{
    public class Manager : MonoBehaviour
    {
        private Dictionary<string, List<Invesment>> invesments;

        private void Start()
        {
            Subscribe();
            IDProperties();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        [MoonSharpHidden]
        public void Initialize()
        {
            LoadTypes();
            LoadInvesments();
        }

        #region HANDLERS
        private void HandleDayPass(ushort day)
        {
            HandleTimePass("HandleDayPass", day);
        }

        private void HandleMonthPass(ushort month)
        {
            HandleTimePass("HandleMonthPass", month);
        }

        private void HandleYearPass(ushort year)
        {
            HandleTimePass("HandleYearPass", year);
        }

        private void HandleTimePass(string eventName, ushort timePass)
        {
            foreach (var type in invesments.Keys)
            {
                Script script = Types.Dictionary[type].script;
                if (script.Globals[eventName] != null)
                {
                    Debug.Log($"Lua start time {UnityEngine.Time.time}");
                    script.Call(script.Globals[eventName], timePass, invesments[type]);
                    Debug.Log($"Lua end time {UnityEngine.Time.time}");
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
        private void LoadTypes()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/invesment/types/types.json");
            if (json == null) return;

            Types.LoadJson(json);
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
            ulong i = 0;
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
