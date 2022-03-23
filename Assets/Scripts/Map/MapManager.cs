using System;
using System.IO;
using System.Linq;
using UnityEngine;
using _District = District;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Map
{
    [MoonSharpUserData]
    public class MapManager : Manager.Manager
    {
        public static MapManager Instance { get; private set; }

        public event EventHandler<Transform> OnMapLoaded;
        public event EventHandler<Dictionary<string, District>> OnDistrictsLoaded;

        public event EventHandler<District> OnDistrictClicked;
        public event EventHandler<District> OnDistrictDoubleClicked;

        public Material buildingOutlineMaterial;

        private Dictionary<string, District> districts;

        private GameObject mapObject;
        private Transform districtsTransform;

        private Property lastSelectedProperty;
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

            RaiseOnRulesLoaded();
        }

        public void HideActiveDistrict()
        {
            if (lastOpenDistrict != null)
            {
                lastOpenDistrict.Hide();
                lastOpenDistrict = null;
            }
        }

        #region HANDLERS
        private void HandleCameraZoomed(object sender, float zoom)
        {
            buildingOutlineMaterial.SetFloat("_FirstOutlineWidth", Mathf.Max(0.3f, zoom*2));
        }

        private void HandleMouseOverProperty(object sender, Property property)
        {
            
            buildingOutlineMaterial.SetTexture("_MainTex", property.Texture);
            property.Outline(buildingOutlineMaterial);
        }

        private void HandlePropertySelected(object sender, Property property)
        {
            if (lastSelectedProperty != null && lastSelectedProperty != property)
            {
                lastSelectedProperty.Deselect();
            }
            property.Outline(buildingOutlineMaterial);
            lastSelectedProperty = property;
        }

        private void HandlePropertyDeselected(object sender, Property property)
        {
            property.Deoutline();
        }

        private void HandleDistrictClick(object sender, District district)
        {
            OnDistrictClicked?.Invoke(this, district);

            if (lastOpenDistrict != district)
            {
                HideActiveDistrict();
            }
            lastOpenDistrict = district;
        }

        private void HandleDistrictDoubleClick(object sender, District district)
        {
            OnDistrictDoubleClicked?.Invoke(this, district);

        }
        
        private void HandleDistrictDataLoaded(object sender, Dictionary<string, _District.Data> datas)
        {
            foreach (var key in districts.Keys)
            {
                districts[key].UpdateName(datas[key].name);
            }
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
                    Path.Combine(Application.streamingAssetsPath, "vanilla/map/ui/districtui"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("DistrictUI");
            districts = new Dictionary<string, District>();

            foreach (Transform district in districtsTransform)
            {
                if (!districts.ContainsKey(district.name))
                {
                    CreateDistrictUI(prefab, district);

                    District districtClass = district.gameObject.AddComponent<District>();
                    districtClass.OnClicked += HandleDistrictClick;
                    districtClass.OnDoubleClicked += HandleDistrictDoubleClick;
                    districtClass.OnMouseOverProperty += HandleMouseOverProperty;
                    districtClass.OnPropertySelected += HandlePropertySelected;
                    districtClass.OnPropertyDeselected += HandlePropertyDeselected;
                    districts.Add(district.name, districtClass);
                }
            }

            OnDistrictsLoaded?.Invoke(this, districts);
        }

        private void CreateDistrictUI(GameObject prefab, Transform district)
        {
            //Transform model = district.Find("Model");
            // model.localScale = new Vector3(model.localScale.x, 0.001f, model.localScale.z);

            if (district.name == "Dummy") return;
            if (!district.gameObject.activeInHierarchy) return;

            Transform hitbox = district.Find("Hitbox").GetChild(0);
            if (hitbox == null) return;

            GameObject districtUI = Instantiate(prefab, district);
            districtUI.layer = LayerMask.NameToLayer("WorldCanvas");
            districtUI.GetComponent<Canvas>().sortingOrder = 99;
            districtUI.transform.localPosition = hitbox.localPosition + new Vector3(0, .5f, 0);
            districtUI.transform.localRotation = Quaternion.Euler(90, 0, -90);
            districtUI.name = "Canvas";

            float width = districtUI.GetComponent<RectTransform>().sizeDelta.x;
            Vector3 size = hitbox.GetComponent<Renderer>().bounds.size;
            size = new Vector3(size.z / 2f, size.x / 10f, size.y);
            float scale = size.x / width * 1.5f;
            districtUI.transform.localScale = new Vector3(scale, scale, 1);
        }
        #endregion

        #region SUBSCRIPTIONS
        protected override void Subscribe()
        {
            base.Subscribe();

            _District.DistrictManager.Instance.OnDistrictDataLoaded += HandleDistrictDataLoaded;
            Chess.Camera.CameraController.Singleton.OnCameraZoomed += HandleCameraZoomed;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();

            _District.DistrictManager.Instance.OnDistrictDataLoaded -= HandleDistrictDataLoaded;
            Chess.Camera.CameraController.Singleton.OnCameraZoomed -= HandleCameraZoomed;
        }
        #endregion

        #region MISC
        [ContextMenu("Set Districts According to the Current Map")]
        public void PopulateDistricts()
        {
            foreach (_District.Data data in _District.Data.Datas.Values)
            {
                if (districts.TryGetValue(data.tag, out District district))
                {
                    data.size = district.transform.Find("Buildings/LOD0").childCount;
                }
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(_District.Data.Datas.Values.OrderBy(x => x.tag).ToList(), Newtonsoft.Json.Formatting.Indented);
            Utils.StreamingAssetsHandler.SafeSetString($"vanilla/district/districts.json", json);
        }
        #endregion
    }
}
