using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;

namespace Map
{
    [MoonSharpUserData]
    public class MapManager : Manager.Manager
    {
        public static MapManager Instance { get; private set; }

        public event EventHandler<Transform> OnMapLoaded;
        public event EventHandler<Dictionary<string, Transform>> OnDistrictsLoaded;

        public event EventHandler<District> OnDistrictClicked;
        public event EventHandler<District> OnDistrictDoubleClicked;


        private Dictionary<string, Transform> districts;

        private GameObject mapObject;
        private Transform districtsTransform;


        private District lastOpenDistrict;

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
            LoadDistricts();
        }

        #region HANDLERS
        private void HandleDistrictClick(object sender, District district)
        {
            OnDistrictClicked?.Invoke(this, district);

            if (lastOpenDistrict != null && lastOpenDistrict != district)
            {
                lastOpenDistrict.Hide();
            }
            lastOpenDistrict = district;
        }

        private void HandleDistrictDoubleClick(object sender, District district)
        {
            OnDistrictDoubleClicked?.Invoke(this, district);

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
            districtsTransform = mapObject.transform.Find("Districts");

            OnMapLoaded?.Invoke(this, mapObject.transform);
        }

        private void LoadDistricts()
        {
            var myLoadedAssetBundle
                = AssetBundle.LoadFromFile(
                    Path.Combine(Application.streamingAssetsPath, "vanilla/map/ui/district"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("DistrictUI");
            districts = new Dictionary<string, Transform>();
            foreach (Transform district in districtsTransform)
            {
                if (!districts.ContainsKey(district.name))
                {
                    CreateDistrictUI(prefab, district);

                    districts.Add(district.name, district);
                    District districtClass = district.gameObject.AddComponent<District>();
                    districtClass.OnClicked += HandleDistrictClick;
                    districtClass.OnDoubleClicked += HandleDistrictDoubleClick;
                }
            }

            OnDistrictsLoaded?.Invoke(this, districts);
        }

        private void CreateDistrictUI(GameObject prefab, Transform district)
        {
            Transform model = district.Find("Model");
            model.localScale = new Vector3(model.localScale.x, 0.001f, model.localScale.z);

            GameObject districtUI = Instantiate(prefab, district);
            districtUI.layer = LayerMask.NameToLayer("WorldCanvas");
            districtUI.GetComponent<Canvas>().sortingOrder = 99;
            districtUI.transform.localPosition = model.localPosition + new Vector3(0, .5f, 0);
            districtUI.transform.localRotation = Quaternion.Euler(90, 0, -120);
            districtUI.name = "Canvas";

            float width = districtUI.GetComponent<RectTransform>().sizeDelta.x;
            Vector3 size = model.GetComponent<Renderer>().bounds.size;
            size = new Vector3(size.z / 2f, size.x / 10f, size.y);
            float scale = size.x / width * 1.5f;
            districtUI.transform.localScale = new Vector3(scale, scale, 1);
        }
        #endregion
    }
}
