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

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "ui/manager.lua";

            LoadScript();
            LoadElements();
        }

        #region CONTENT LOADER
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
            Script script = Utils.StreamingAssetsHandler.SafeGetScript($"vanilla/ui/elements/{name}/{name}.lua");
            if (script == null) return null;

            script.Globals["Instantiate"] = (Func<UnityEngine.Object, Transform, UnityEngine.Object>)Instantiate;
            script.Globals["Color"] = (Func<float, float, float, float, Color>)((r, g, b, a) => { return new Color(r, g, b, a); });

            script.Call(script.Globals[nameof(OnScriptLoaded)]);
            return script;
        }
        #endregion
    }
}

