using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Load
{
    [MoonSharpUserData]
    public class LoadManager : Manager.Manager
    {
        public static LoadManager Instance { get; private set; }

        public event EventHandler<Progress> OnLoadProgressed;
        public event EventHandler OnProgressStart;
        public event EventHandler OnProgressFinish;
        public event EventHandler OnGameRulesLoaded;
        public event EventHandler OnGameContentLoaded;

        public List<Manager.Manager> managers;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        protected override void Start()
        {
            base.Start();
            LoadManagerRules();
        }

        private void LoadManagerRules()
        {
            StartCoroutine(LoadManagerRulesCoroutine());
        }

        private IEnumerator LoadManagerRulesCoroutine()
        {
            OnProgressStart?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("LOGIC_START", 0f);

            float delta;
            foreach (var manager in managers)
            {
                delta = UnityEngine.Time.deltaTime;
                yield return RaiseLoadProgressed($"{manager.GetType().Name.ToUpper()}_LOGIC", delta);
                // yield return new WaitForSeconds(0.1f);
                manager.LoadRules();
            }

            OnProgressFinish?.Invoke(this, EventArgs.Empty);
            OnGameRulesLoaded?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("LOGIC_READY", UnityEngine.Time.deltaTime);
        }

        private void LoadManagerContent(string path)
        {
            StartCoroutine(LoadManagerContentCoroutine(path));
        }

        private IEnumerator LoadManagerContentCoroutine(string path)
        {
            OnProgressStart?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("CONTENT_START", 0f);

            float delta;
            foreach (var manager in managers)
            {
                delta = UnityEngine.Time.deltaTime;
                yield return RaiseLoadProgressed($"{manager.GetType().Name.ToUpper()}_CONTENT", delta);
                // yield return new WaitForSeconds(0.1f);
                manager.LoadContent(path);
            }

            OnProgressFinish?.Invoke(this, EventArgs.Empty);
            OnGameContentLoaded?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("CONTENT_READY", UnityEngine.Time.deltaTime);
        }

        private void HandleSaveFileLoadRequest(object sender, SaveFile.Data data)
        {
            LoadManagerContent(data.path);
        }

        private void HandleScenarioDatasLoaded(object sender, List<SaveFile.Data> datas)
        {
            if (datas == null || datas.Count == 0)
            {
                Console.Console.Run("log_error Scenario files is empty, please check game file integrity through Steam.");
                return;
            }

            LoadManagerContent(datas[0].path);
        }

        [MoonSharpHidden]
        public override void LoadRules() { }

        private bool RaiseLoadProgressed(string msg, float delta)
        {
            OnLoadProgressed?.Invoke(this, new Progress(msg, delta));
            return true;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            SaveFile.SaveFileManager.Instance.OnDataFileLoadRequested += HandleSaveFileLoadRequest;
            SaveFile.SaveFileManager.Instance.OnScenarioDatasLoaded += HandleScenarioDatasLoaded;
        }

        protected override void Unsubscribe()
        {
            base.Subscribe();
            SaveFile.SaveFileManager.Instance.OnDataFileLoadRequested -= HandleSaveFileLoadRequest;
            SaveFile.SaveFileManager.Instance.OnScenarioDatasLoaded -= HandleScenarioDatasLoaded;
        }
    }
}

