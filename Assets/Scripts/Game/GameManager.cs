using System;
using MoonSharp.Interpreter;

namespace Game
{
    [MoonSharpUserData]
    public class GameManager : Manager.Manager
    {
        public static GameManager Instance { get; private set; }

        public float ScriptUpdateInterval = -1f;
        private float lastScriptUpdate = 0f;
        private float scirptUpdateDelta = 0f;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            ScriptUpdateInterval = -1f;

            scriptPath = "game/manager.lua";
            LoadScript();
        }

        private void Update()
        {
            scirptUpdateDelta += UnityEngine.Time.deltaTime;
            if (ScriptUpdateInterval != -1 && UnityEngine.Time.time >= lastScriptUpdate + ScriptUpdateInterval)
            {
                script.Call(script.Globals["OnUpdate"], scirptUpdateDelta);
                lastScriptUpdate = UnityEngine.Time.time;
                scirptUpdateDelta = 0;
            }
        }
    }
}
