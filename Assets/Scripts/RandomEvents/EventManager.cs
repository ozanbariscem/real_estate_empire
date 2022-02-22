using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace RandomEvent
{
    [MoonSharpUserData]
    public class EventManager : Manager.Manager
    {
        public static EventManager Instance { get; private set; }

        public event EventHandler OnEventsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        public void FireEvent(string tag)
        {
            if (EventDictionary.Dictionary.TryGetValue(tag, out Script script))
            {
                script.Call(script.Globals["OnEventFired"]);
            }
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "events/manager.lua";

            LoadScript();
            LoadEvents();
            RaiseOnRulesLoaded();
        }

        private void LoadEvents()
        {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "vanilla/events/scripts"));

            Dictionary<string, Script> events = new Dictionary<string, Script>();
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Extension == ".lua")
                {
                    string name = Path.GetFileNameWithoutExtension(file.Name);

                    Script script = Utils.ContentHandler.SafeGetScript(file.FullName);
                    script.Globals["tag"] = name;
                    script.Call(script.Globals["OnScriptLoaded"]);
                    events.Add(name, script);
                }
            }

            EventDictionary.LoadEvents(events);
            OnEventsLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
