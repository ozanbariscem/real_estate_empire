using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Game
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;
        public event Action<Game.Manager, Time.Manager, Map.Manager> OnManagersInitialized;

        [SerializeField] 
        private Time.Manager timeManager;
        [SerializeField]
        private Map.Manager mapManager;

        private Script script;
        public float ScriptUpdateInterval = -1f;
        private float lastScriptUpdate = 0f;
        private float scirptUpdateDelta = 0f;

        // It's safe to call this because Game.Manager.Start() is ran the last
        private void Start()
        {
            ScriptUpdateInterval = -1f;

            LoadScript();

            if (timeManager)
                timeManager.Initialize();
            else
                Debug.LogError("Time manager is not referenced. This will cause major problems.");
            if (mapManager)
                mapManager.Initialize();
            else
                Debug.LogError("Map manager is not referenced. This will cause major problems.");


            OnManagersInitialized?.Invoke(this, timeManager, mapManager);
            script.Call(script.Globals[nameof(OnManagersInitialized)], this, timeManager, mapManager);
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

        #region CONTENT LOADER
        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/game/manager.lua");
            if (scriptString == null) return;

            UserData.RegisterType<Game.Manager>();
            UserData.RegisterType<Time.Manager>();
            UserData.RegisterType<Map.Manager>();

            script = new Script();
            script.Globals["Log"] = (Action<string>)Debug.Log;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke();
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }
        #endregion
    }
}
