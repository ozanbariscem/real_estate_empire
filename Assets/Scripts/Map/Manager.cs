using System;
using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Map
{
    public class Manager : MonoBehaviour
    {
        public event Action OnScriptLoaded;
        public event Action<Transform> OnMapLoaded;
        public event Action<Transform[]> OnBuildingsLoaded;

        private Transform[] buildings;

        private GameObject mapObject;
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
            SetBuildings();
        }

        #region SETUP
        private void SetBuildings()
        {
            int childCount = mapObject.transform.childCount;

            buildings = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                buildings[i] = mapObject.transform.GetChild(i);
            }
            OnBuildingsLoaded?.Invoke(buildings);
            script.Call(script.Globals[nameof(OnBuildingsLoaded)], buildings);
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
