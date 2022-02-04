using System;
using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Language
{
    public class Manager : MonoBehaviour
    {
        public event EventHandler OnScriptLoaded;

        private Dictionary<string, Dictionary> languages;
        private string selectedLanguage;

        private Script script;

        [MoonSharpHidden]
        public void Initialize()
        {
            LoadScript();
            LoadLanguages();
            ChangeLanguage("en");
        }

        public string Translate(string tag)
        {
            return languages[selectedLanguage].Get(tag);
        }

        public void ChangeLanguage(string tag)
        {
            if (languages.ContainsKey(tag))
                selectedLanguage = tag;
        }

        #region CONTENT LOADER
        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/language/manager.lua");
            if (scriptString == null) return;

            UserData.RegisterType<Console.Console>();

            script = new Script();
            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke(this, EventArgs.Empty);
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }

        private void LoadLanguages()
        {
            languages = new Dictionary<string, Dictionary>();

            string path = Path.Combine(Application.streamingAssetsPath, "vanilla/language/languages");

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (file.Contains(".meta")) continue;

                FileInfo fileInfo = new FileInfo(file);
                string name = Path.GetFileNameWithoutExtension(fileInfo.Name);

                if (File.Exists(fileInfo.FullName))
                {
                    string json = Utils.StreamingAssetsHandler.SafeGetString(file);
                    if (json == null) continue;

                    Dictionary<string, string> translation = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    
                    languages.Add(name, new Dictionary(translation));
                }
            }
        }
        #endregion
    }
}
