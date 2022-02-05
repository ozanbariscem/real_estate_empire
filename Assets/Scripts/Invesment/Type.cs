using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Invesment
{
    [Serializable]
    [MoonSharpUserData]
    public class Type
    {
        public string type;
        [JsonIgnore]
        public Script script;
        [JsonProperty("script")]
        public string scriptPath;
        public Dictionary<string, Data> subTypes;

        public Type(string type, string scriptPath, Dictionary<string, Data> subTypes)
        {
            this.type = type;
            this.subTypes = subTypes;
            this.scriptPath = scriptPath;

            LoadScript();
        }

        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/invesment/types/{scriptPath}");
            if (scriptString == null) return;

            UserData.RegisterType<Data>();
            UserData.RegisterType<Type>();
            UserData.RegisterType<Invesment>();

            script = new Script();
            script.Globals["Log"] = (Action<string>)Debug.Log;
            script.DoString(scriptString);

            script.Call(script.Globals["OnReady"]);
        }
    }
}

