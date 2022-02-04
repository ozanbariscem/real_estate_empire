using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;

namespace UI
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;

        private Game.Manager gameManager;

        private Script script;

        [MoonSharpHidden]
        public void Initialize(Game.Manager gameManager)
        {
            this.gameManager = gameManager;
            LoadScript();
            LoadElements();
        }

        #region SUBSCRIPTIONS
        #endregion

        #region CONTENT LOADER
        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/ui/manager.lua");
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
            script.Globals["GameManager"] = gameManager;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke();
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }

        private void LoadElements()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "vanilla/ui/elements");

            string[] directories = Directory.GetDirectories(path);

            foreach (string directory in directories)
            {
                string name = new DirectoryInfo(directory).Name;
                string assetBundleFile = Path.Combine(directory, name);

                if (File.Exists(assetBundleFile))
                {
                    AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleFile);
                    if (assetBundle == null)
                    {
                        Console.Console.Run($"log_error Failed to load AssetBundle ui/elements/{name}!");
                        return;
                    }
                    GameObject prefab = assetBundle.LoadAsset<GameObject>(name);
                    GameObject obj = Instantiate(prefab, transform);

                    Element element = obj.AddComponent<Element>();
                    element.SetScript(LoadElementScript(name));
                }
            }
        }

        private Script LoadElementScript(string name)
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString($"vanilla/ui/elements/{name}/{name}.lua");
            if (scriptString == null) return null;

            UserData.RegisterType<TMPro.TextMeshProUGUI>();
            UserData.RegisterType<Transform>();
            UserData.RegisterType<GameObject>();
            UserData.RegisterType<Image>();
            UserData.RegisterType<RawImage>();
            UserData.RegisterType<Color>();
            UserData.RegisterType<Texture2D>();

            UserData.RegisterType<EventArgs>();

            UserData.RegisterType<Game.Manager>();
            UserData.RegisterType<Time.Manager>();
            UserData.RegisterType<Map.Manager>();
            UserData.RegisterType<Invesment.Manager>();
            UserData.RegisterType<Investor.Manager>();
            UserData.RegisterType<Ownership.Manager>();
            UserData.RegisterType<Language.Manager>();
            UserData.RegisterType<UI.Manager>();
            UserData.RegisterType<Console.Console>();

            UserData.RegisterType<Time.Date>();
            UserData.RegisterType<Map.Invesment>();
            UserData.RegisterType<Investor.Investor>();
            UserData.RegisterType<Invesment.Invesment>();
            UserData.RegisterType<Ownership.Ownership>();

            UserData.RegisterType<Investor.InvestorList>();
            UserData.RegisterType<Invesment.InvesmentDictionary>();
            UserData.RegisterType<Ownership.OwnershipList>();

            Script script = new Script();
            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;
            script.Globals["Instantiate"] = (Func<UnityEngine.Object, Transform, UnityEngine.Object>)Instantiate;
            script.Globals["GameManager"] = gameManager;

            script.Globals["Color"] = (Func<float, float, float, float, Color>)((r, g, b, a) => { return new Color(r, g, b, a); });

            script.Globals["InvesmentDictionary"] = new Invesment.InvesmentDictionary();
            script.Globals["OwnershipList"] = new Ownership.OwnershipList();
            script.Globals["InvestorList"] = new Investor.InvestorList();

            script.DoString(scriptString);

            script.Call(script.Globals[nameof(OnScriptLoaded)]);
            return script;
        }
        #endregion
    }
}

