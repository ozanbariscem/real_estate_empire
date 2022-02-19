using System;
using UnityEngine;
using MoonSharp.Interpreter;
using Investment;
using Time;
using Game;
using Investor;
using UI;
using Language;
using Map;
using Ownership;

namespace Manager
{
    [Serializable]
    [MoonSharpUserData]
    public abstract class Manager : MonoBehaviour
    {
        public event EventHandler OnScriptLoaded;
        public event EventHandler OnRulesLoaded;
        public event EventHandler OnContentLoaded;

        protected string scriptPath;

        [MoonSharpHidden]
        public Script script;

        protected virtual void Start()
        {
            Subscribe();
        }

        protected virtual void OnDestroy()
        {
            Unsubscribe();
        }

        [MoonSharpHidden]
        public virtual void LoadRules()
        {
            LoadScript();

            RaiseOnRulesLoaded();
        }

        /// <summary>
        /// Don't forget to RaiseOnContentLoaded if you override this
        /// </summary>
        [MoonSharpHidden]
        public virtual void LoadContent(string path)
        {
            RaiseOnContentLoaded();
        }

        protected virtual void RaiseOnScriptLoaded()
        {
            OnScriptLoaded?.Invoke(this, EventArgs.Empty);
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }

        protected virtual void RaiseOnRulesLoaded()
        {
            OnRulesLoaded?.Invoke(this, EventArgs.Empty);
            script.Call(script.Globals[nameof(OnRulesLoaded)]);
        }
        protected virtual void RaiseOnContentLoaded()
        {
            OnContentLoaded?.Invoke(this, EventArgs.Empty);
            script.Call(script.Globals[nameof(OnContentLoaded)]);
        }

        #region CONTENT LOADING
        protected virtual void LoadScript()
        {
            script = Utils.StreamingAssetsHandler.SafeGetScript($"vanilla/{scriptPath}");
            if (script != null)
                RaiseOnScriptLoaded();
        }
        #endregion

        #region SUBSCRIPTIONS
        protected virtual void Subscribe() { }
        protected virtual void Unsubscribe() { }
        #endregion
    }
}
