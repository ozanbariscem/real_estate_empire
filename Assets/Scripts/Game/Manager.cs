using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Game
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;
        public event Action<
            Game.Manager,
            Time.Manager, Map.Manager,
            Invesment.Manager, Investor.Manager,
            Ownership.Manager, UI.Manager> OnManagersInitialized;

        [SerializeField]
        private Time.Manager timeManager;
        [SerializeField]
        private Map.Manager mapManager;
        [SerializeField]
        private Invesment.Manager invesmentManager;
        [SerializeField]
        private Investor.Manager investorManager;
        [SerializeField]
        private Ownership.Manager ownershipManager;
        [SerializeField]
        private UI.Manager uiManager;

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
            {
                timeManager.Initialize();
            }
            else
                Debug.LogError("Time manager is not referenced. This will cause major problems.");
            if (mapManager)
            {
                mapManager.Initialize();
                mapManager.OnInvesmentClicked += HandleOnInvesmentClicked;
            }
            else
                Debug.LogError("Map manager is not referenced. This will cause major problems.");
            if (invesmentManager)
            {
                invesmentManager.Initialize();
            }
            else
                Debug.LogError("Invesment manager is not referenced. This will cause major problems.");
            if (investorManager)
            {
                investorManager.Initialize();
            }
            else
                Debug.LogError("Investor manager is not referenced. This will cause major problems.");
            if (ownershipManager)
            {
                ownershipManager.Initialize();
            }
            else
                Debug.LogError("Ownership manager is not referenced. This will cause major problems."); 
            if (uiManager)
            {
                uiManager.Initialize(this);
            }
            else
                Debug.LogError("UI manager is not referenced. This will cause major problems.");


            OnManagersInitialized?.Invoke(
                this, timeManager, mapManager, invesmentManager, investorManager, ownershipManager, uiManager);
            script.Call(
                script.Globals[nameof(OnManagersInitialized)], this,
                timeManager, mapManager, invesmentManager, investorManager, ownershipManager, uiManager);
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

        #region GETTERS
        public Time.Manager GetTimeManager() => timeManager;
        public Map.Manager GetMapManager() => mapManager;
        public Invesment.Manager GetInvesmentManager() => invesmentManager;
        public Investor.Manager GetInvestorManager() => investorManager;
        public Ownership.Manager GetOwnershipManager() => ownershipManager;
        public UI.Manager GetUIManager() => uiManager;
        #endregion

        #region HANDLERS
        public void HandleOnInvesmentClicked(string tag, int id)
        {
            Invesment.Invesment invesment = Invesment.InvesmentDictionary.SafeGetInvesment(tag, id);
            if (invesment != null)
            {
                Console.Console.Run($"log " +
                    $"Invesment: [{tag} {id}] -> [{invesment.type} {invesment.id}], {invesment}");
            }
            else
            {
                Console.Console.Run($"log_error Can't find invesment with tag-id: {tag}-{id}. Please report this with id to us or the modders if you use any mods.");
            }
        }
        #endregion

        #region CONTENT LOADER
        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/game/manager.lua");
            if (scriptString == null) return;

            UserData.RegisterType<Game.Manager>();
            UserData.RegisterType<Time.Manager>();
            UserData.RegisterType<Map.Manager>();
            UserData.RegisterType<Invesment.Manager>();
            UserData.RegisterType<Investor.Manager>();
            UserData.RegisterType<Ownership.Manager>();
            UserData.RegisterType<UI.Manager>();
            UserData.RegisterType<Console.Console>();

            script = new Script();
            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke();
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }
        #endregion
    }
}
