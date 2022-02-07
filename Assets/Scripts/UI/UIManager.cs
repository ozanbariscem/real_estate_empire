using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;

namespace UI
{
    [MoonSharpUserData]
    public class UIManager : Manager.Manager
    {
        public static UIManager Instance { get; private set; }

        public Transform consoleMenu;
        public Transform mainMenu;
        public Transform loadMenu;
        public Transform gameMenu;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        protected override void Start()
        {
            base.Start();
            LoadLoadMenu();
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "ui/manager.lua";

            LoadScript();
            LoadMainMenu();
            LoadElements();
        }

        #region CONTENT LOADER
        /// <summary>
        /// This is a special condition that should be handled seperatly
        /// </summary>
        private void LoadMainMenu()
        {
            string name = "main_menu";
            string assetBundleFile = Path.Combine(Application.streamingAssetsPath, $"vanilla/ui/main_menu/{name}");

            if (File.Exists(assetBundleFile))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleFile);
                if (assetBundle == null)
                {
                    Console.Console.Run($"log_error Failed to load AssetBundle ui/main_menu/{name}!");
                    return;
                }
                GameObject prefab = assetBundle.LoadAsset<GameObject>("MainMenu");
                GameObject obj = Instantiate(prefab, mainMenu);
                obj.name = "MainMenu";

                Element element = obj.AddComponent<Element>();
                element.SetScript(LoadElementScript($"{name}/{name}"));
            }
        }

        /// <summary>
        /// This is a special condition that should be handled seperatly
        /// </summary>
        private void LoadLoadMenu()
        {
            string name = "loading_menu";
            string assetBundleFile = Path.Combine(Application.streamingAssetsPath, $"vanilla/ui/loading_menu/{name}");

            if (File.Exists(assetBundleFile))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleFile);
                if (assetBundle == null)
                {
                    Console.Console.Run($"log_error Failed to load AssetBundle ui/loading_menu/{name}!");
                    return;
                }
                GameObject prefab = assetBundle.LoadAsset<GameObject>("LoadingMenu");
                GameObject obj = Instantiate(prefab, loadMenu);
                obj.name = "LoadingMenu";

                Element element = obj.AddComponent<Element>();
                element.SetScript(LoadElementScript($"{name}/{name}"));
            }
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
                    GameObject obj = Instantiate(prefab, gameMenu);

                    Element element = obj.AddComponent<Element>();
                    element.SetScript(LoadElementScript($"elements/{name}/{name}"));
                }
            }
        }

        private Script LoadElementScript(string path)
        {
            Script script = Utils.StreamingAssetsHandler.SafeGetScript($"vanilla/ui/{path}.lua");
            if (script == null) return null;

            script.Globals["AddFunctionality"] = (Action<Transform, string, object>)((transform, function, args) =>
            {
                transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    script.Call(script.Globals[function], args);
                });
            });
            script.Globals["Instantiate"] = (Func<UnityEngine.Object, Transform, UnityEngine.Object>)Instantiate;
            script.Globals["Color"] = (Func<float, float, float, float, Color>)((r, g, b, a) => { return new Color(r, g, b, a); });

            script.Globals["Quit"] = (Action)Application.Quit;

            script.Call(script.Globals[nameof(OnScriptLoaded)]);
            return script;
        }
        #endregion
    }
}

