using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

        public Transform debugMenu;

        private List<Element> openMenus;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PopLastMenu();
            }
        }

        public Transform OpenMenu(string path, object param)
        {
            Transform target = transform.Find(path);

            if (target != null)
            {
                Element element = target.GetComponent<Element>();

                if (element != null)
                {
                    target.SetSiblingIndex(target.parent.childCount - 1);
                    element.Activate(param);

                    if (openMenus == null)
                        openMenus = new List<Element>();
                    openMenus.Add(element);
                }
            }

            return target;
        }

        public void BringToFront(Element element)
        {
            for (int i = openMenus.Count -1; i >= 0; i--)
            {
                if (openMenus[i] == element)
                {
                    openMenus.RemoveAt(i);
                    openMenus.Add(element);
                    element.transform.SetAsLastSibling();
                    break;
                }
            }
        }

        private void PopLastMenu()
        {
            for (int i = openMenus.Count - 1; i >= 0; i--)
            {
                Element menu = openMenus[i];
                openMenus.RemoveAt(i);

                if (menu.gameObject.activeInHierarchy)
                {
                    menu.Deactivate();
                    break;
                }
            }
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "ui/manager.lua";

            LoadScript();
            LoadMainMenu();
            LoadElements();

            RaiseOnRulesLoaded();
        }

        private void HandleDebugMenuToggled(object sender, REE.Debug.State state)
        {
            debugMenu.gameObject.SetActive(state.IsActive);
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
                    obj.name = name;

                    if (name == "hover")
                        obj.AddComponent<Hover>();

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
                Button button = transform.GetComponent<Button>();

                if (button != null)
                {
                    button.onClick.AddListener(
                        () => script.Call(script.Globals[function], args));
                }
                else
                {
                    EventTrigger eventTrigger = transform.gameObject.AddComponent<EventTrigger>();

                    EventTrigger.Entry onClick = new EventTrigger.Entry();
                    onClick.eventID = EventTriggerType.PointerClick;
                    onClick.callback.AddListener((eventData) =>
                    {
                        script.Call(script.Globals[function], args);
                    });

                    eventTrigger.triggers.Add(onClick);
                }
            });
            script.Globals["Instantiate"] = (Func<UnityEngine.Object, Transform, UnityEngine.Object>)Instantiate;
            script.Globals["Color"] = (Func<float, float, float, float, Color>)((r, g, b, a) => { return new Color(r, g, b, a); });
            script.Globals["Log"] = (Action<string>)Debug.Log;

            script.Globals["Quit"] = (Action)Application.Quit;

            script.Call(script.Globals[nameof(OnScriptLoaded)]);
            return script;
        }
        #endregion

        protected override void Subscribe()
        {
            base.Subscribe();
            REE.Debug.Debug.OnToggled += HandleDebugMenuToggled;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            REE.Debug.Debug.OnToggled -= HandleDebugMenuToggled;
        }
    }
}

