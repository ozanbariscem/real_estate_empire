using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Language
{
    [MoonSharpUserData]
    public class LanguageManager : Manager.Manager
    {
        public static LanguageManager Instance { get; private set; }

        private Dictionary<string, Dictionary> languages;
        private string selectedLanguage;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "language/manager.lua";
            LoadScript();
            LoadLanguages();
            ChangeLanguage("en");

            RaiseOnRulesLoaded();
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
