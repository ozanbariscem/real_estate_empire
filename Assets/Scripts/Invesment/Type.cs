using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Investment
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
            script = Utils.StreamingAssetsHandler.SafeGetScript($"vanilla/investment/types/{scriptPath}");
            if (script != null)
            {
                script.Call(script.Globals["OnReady"]);
            }
        }
    }
}

