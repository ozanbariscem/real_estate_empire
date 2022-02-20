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

        public List<string> modifier_groups;

        public Type(string type, string scriptPath, Dictionary<string, Data> subTypes, List<string> modifier_groups)
        {
            this.type = type;
            this.subTypes = subTypes;
            this.scriptPath = scriptPath;
            this.modifier_groups = modifier_groups;

            LoadScript();
        }

        private void LoadScript()
        {
            script = Utils.StreamingAssetsHandler.SafeGetScript($"vanilla/investment/types/{scriptPath}");
            if (script != null)
            {
                script.Globals["modifier_groups"] = modifier_groups;
                script.Call(script.Globals["OnReady"]);
            }
        }
    }
}

