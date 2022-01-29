using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Map
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;
        public event Action<Transform> OnMapLoaded;
        public event Action<Dictionary<string, Transform[]>> OnInvesmentsLoaded;

        public event Action<string, int> OnInvesmentClicked;

        private Dictionary<string, Transform[]> invesments;

        private GameObject mapObject;
        private Transform invesmentsObject;
        private Script script;

        // Only use for event subscriptions
        private void Start() {}

        /// <summary>
        /// Handled by the Game.Manager
        /// </summary>
        [MoonSharpHidden]
        public void Initialize()
        {
            LoadScript();
            LoadMap();
            LoadInvesments();
        }

        public void HandleInvesmentClicked(string tag, int id)
        {
            OnInvesmentClicked?.Invoke(tag, id);
            script.Call(script.Globals[nameof(OnInvesmentClicked)], tag, id);
        }

        #region SETUP
        private void LoadInvesments()
        {
            int childCount = invesmentsObject.childCount;

            invesments = new Dictionary<string, Transform[]>();
            for (int i = 0; i < childCount; i++)
            {
                string name = invesmentsObject.GetChild(i).name;

                if (!invesments.ContainsKey(name))
                {
                    int invesmentCount = invesmentsObject.GetChild(i).childCount;

                    invesments.Add(name, new Transform[invesmentCount]);

                    for (int j = 0; j < invesmentCount; j++)
                    {
                        invesments[name][j] = invesmentsObject.GetChild(i).GetChild(j);
                        SetInvesment(invesments[name][j], name, j);
                    }
                }
            }

            OnInvesmentsLoaded?.Invoke(invesments);
            script.Call(script.Globals[nameof(OnInvesmentsLoaded)], invesments);
        }

        private void SetInvesment(Transform transform, string tag, int id)
        {
            // Add collider
            // Add building script
            // That's it
            transform.gameObject.AddComponent<PolygonCollider2D>();
            Invesment invesment = transform.gameObject.AddComponent<Invesment>();
            invesment.Set(tag, id);
            invesment.OnInvesmentClicked += HandleInvesmentClicked;
        }
        #endregion

        #region CONTENT LOADER
        private void LoadMap()
        {
            var myLoadedAssetBundle
                = AssetBundle.LoadFromFile(
                    Path.Combine(Application.streamingAssetsPath, "vanilla/map/map"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("Map");
            mapObject = Instantiate(prefab);

            invesmentsObject = mapObject.transform.GetChild(1);

            OnMapLoaded?.Invoke(mapObject.transform);
            script.Call(script.Globals[nameof(OnMapLoaded)], mapObject.transform);
        }

        private void LoadScript()
        {
            string scriptString = Utils.StreamingAssetsHandler.SafeGetString("vanilla/map/manager.lua");
            if (scriptString == null) return;

            UserData.RegisterType<Transform>();

            script = new Script();
            script.Globals["Log"] = (Action<string>)Debug.Log;
            script.DoString(scriptString);

            OnScriptLoaded?.Invoke();
            script.Call(script.Globals[nameof(OnScriptLoaded)]);
        }
        #endregion
    }
}
