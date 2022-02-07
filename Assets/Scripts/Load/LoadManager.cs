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

        public List<Manager.Manager> managers;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Load());
        }

        private IEnumerator Load()
        {
            OnProgressStart?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("Loading game logic...");

            foreach (var manager in managers)
            {
                yield return RaiseLoadProgressed($"Loading rules of {manager.GetType()}");
                // yield return new WaitForSeconds(0.1f);
                manager.LoadRules();
            }

            yield return RaiseLoadProgressed("Loading game content...");

            foreach (var manager in managers)
            {
                yield return RaiseLoadProgressed($"Loading content of {manager.GetType()}");
                // yield return new WaitForSeconds(0.1f);
                manager.LoadContent(System.IO.Path.Combine(Application.streamingAssetsPath, "vanilla"));
            }

            OnProgressFinish?.Invoke(this, EventArgs.Empty);
            yield return RaiseLoadProgressed("Game ready, enjoy!");
        }

        [MoonSharpHidden]
        public override void LoadRules() { }

        private bool RaiseLoadProgressed(string msg)
        {
            OnLoadProgressed?.Invoke(this, new Progress(msg));
            return true;
        }
    }
}

