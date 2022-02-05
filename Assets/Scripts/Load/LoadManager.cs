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
        public event EventHandler<Progress> OnLoadProgressed;

        public List<Manager.Manager> managers;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Load());
        }

        private IEnumerator Load()
        {
            yield return RaiseLoadProgressed("Loading game logic...");

            foreach (var manager in managers)
            {
                yield return RaiseLoadProgressed($"Loading rules of {manager.GetType()}");
                manager.LoadRules();
            }

            yield return RaiseLoadProgressed("Loading game content...");

            foreach (var manager in managers)
            {
                yield return RaiseLoadProgressed($"Loading content of {manager.GetType()}");
                manager.LoadContent(System.IO.Path.Combine(Application.streamingAssetsPath, "vanilla"));
            }

            yield return RaiseLoadProgressed("Game ready, enjoy!");
        }

        [MoonSharpHidden]
        public override void LoadRules() { }

        private void HandleLoadProgressed(object sender, Progress progress)
        {
            Debug.Log(progress.message);
        }

        private bool RaiseLoadProgressed(string msg)
        {
            OnLoadProgressed?.Invoke(this, new Progress(msg));
            return true;
        }

        protected override void Subscribe()
        {
            OnLoadProgressed += HandleLoadProgressed;
        }

        protected override void Unsubscribe()
        {
            OnLoadProgressed -= HandleLoadProgressed;
        }
    }
}

