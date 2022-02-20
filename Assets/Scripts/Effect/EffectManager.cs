using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Newtonsoft.Json;

namespace Effect
{
    [MoonSharpUserData]
    public class EffectManager : Manager.Manager
    {
        public static EffectManager Instance { get; private set; }

        public event EventHandler<Dictionary<Effect.Tags, Effect>> OnEffectsLoaded;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "effects/manager.lua";

            LoadScript();
            LoadEffects();

            RaiseOnRulesLoaded();
        }

        private void LoadEffects()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/effects/effects.json");
            if (json == null) return;

            Effect.LoadEffects(JsonConvert.DeserializeObject<List<Effect>>(json));
            OnEffectsLoaded?.Invoke(this, Effect.Effects);
        }
    }
}
