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
            yield return RaiseLoadProgressed("Loading game logic...");

            foreach (var manager in managers)
            {
                yield return RaiseLoadProgressed($"Loading rules of {manager.GetType()}");
                // yield return new WaitForSeconds(0.1f);
                manager.LoadRules();
            }

            OnProgressFinish?.Invoke(this, EventArgs.Empty);
            OnGameRulesLoaded?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("Game ready, enjoy!");
        }

        private void LoadManagerContent(string path)
        {
            StartCoroutine(LoadManagerContentCoroutine(path));
        }

        private IEnumerator LoadManagerContentCoroutine(string path)
        {
            OnProgressStart?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("Loading game content...");

            foreach (var manager in managers)
            {
                yield return RaiseLoadProgressed($"Loading content of {manager.GetType()}");
                // yield return new WaitForSeconds(0.1f);
                manager.LoadContent(path);
            }

            OnProgressFinish?.Invoke(this, EventArgs.Empty);
            OnGameContentLoaded?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("Game ready, enjoy!");
        }

        private void HandleSaveFileLoadRequest(object sender, SaveFile.Data data)
        {
            LoadManagerContent(data.path);
        }

        [MoonSharpHidden]
        public override void LoadRules() { }

        private bool RaiseLoadProgressed(string msg)
        {
            OnLoadProgressed?.Invoke(this, new Progress(msg));
            return true;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            SaveFile.SaveFileManager.Instance.OnDataFileLoadRequested += HandleSaveFileLoadRequest;
        }

        protected override void Unsubscribe()
        {
            base.Subscribe();
            SaveFile.SaveFileManager.Instance.OnDataFileLoadRequested -= HandleSaveFileLoadRequest;
        }
    }
}

