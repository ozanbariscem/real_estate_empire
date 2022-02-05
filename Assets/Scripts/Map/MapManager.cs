using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Map
{
    [MoonSharpUserData]
    public class MapManager : Manager.Manager
    {
        public static MapManager Instance { get; private set; }

        public event EventHandler<Transform> OnMapLoaded;
        public event EventHandler<Dictionary<string, Transform[]>> OnInvesmentsLoaded;
        public event EventHandler<Invesment> OnInvesmentClicked;

        private Dictionary<string, Transform[]> invesments;

        private GameObject mapObject;
        private Transform invesmentsObject;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        /// <summary>
        /// Handled by the Game.Manager
        /// </summary>
        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "map/manager.lua";

            LoadScript();
            LoadMap();
            LoadInvesments();
        }

        public void HandleInvesmentClicked(object sender, Invesment invesment)
        {
            OnInvesmentClicked?.Invoke(this, invesment);
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
            OnInvesmentsLoaded?.Invoke(this, invesments);
        }

        private void SetInvesment(Transform transform, string tag, int id)
        {
            // Add collider
            // Add building script
            // That's it
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

            OnMapLoaded?.Invoke(this, mapObject.transform);
        }
        #endregion
    }
}
