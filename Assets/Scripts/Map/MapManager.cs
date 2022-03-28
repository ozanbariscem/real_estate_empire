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
        public event EventHandler<Mode> OnMapModeChanged;

        public Settings mapSettings;
        public Mode Mode { get; private set; }

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetMapMode(Mode.Texture);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetMapMode(Mode.Ownership);
        }

        #region MANAGER
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
        #endregion

        #region UTILS
        public void SetMapMode(Mode mode)
        {
            Mode = mode;
            OnMapModeChanged?.Invoke(this, Mode);
        }
        
        public void HideActiveDistrict()
        {
            if (lastOpenDistrict != null)
            {
                lastOpenDistrict.Hide();
                lastOpenDistrict = null;
            }
        }
        #endregion

        #region HANDLERS
        private void HandleCameraZoomed(object sender, float zoom)
        {
            mapSettings.UpdateBorderWidths(zoom);
        }

        private void HandleMouseOverProperty(object sender, Property property)
        {
            if (property != lastSelectedProperty)
            {
                switch (Mode)
                {
                    case Mode.Texture:
                        mapSettings.SetHoveringTexture(property.Texture);
                        mapSettings.SetHoveringColor(Color.white);
                        break;
                    case Mode.Ownership:
                        mapSettings.SetHoveringTexture(null);
                        mapSettings.SetHoveringColor(property.MeshRenderer.sharedMaterial.GetColor("_Color"));
                        break;
                }
                property.Outline(mapSettings.HoverMaterial);
            }
        }

        private void HandleMouseExitedProperty(object sender, Property property)
        {
            if (!property.Selected)
            {
                property.Deoutline(mapSettings.GetMaterialForProperty(Mode, property));
            }
        }

        private void HandlePropertySelected(object sender, Property property)
        {
            if (lastSelectedProperty != null && lastSelectedProperty != property)
            {
                lastSelectedProperty.Deselect();
            }

            switch (Mode)
            {
                case Mode.Texture:
                    mapSettings.SetSelectionTexture(property.Texture);
                    mapSettings.SetSelectionColor(Color.white);
                    break;
                case Mode.Ownership:
                    mapSettings.SetSelectionTexture(null);
                    mapSettings.SetSelectionColor(property.MeshRenderer.sharedMaterial.GetColor("_Color"));
                    break;
            }

            property.Outline(mapSettings.SelectionMaterial);
            lastSelectedProperty = property;
        }

        private void HandlePropertyDeselected(object sender, Property property)
        {
            property.Deoutline(mapSettings.GetMaterialForProperty(Mode, property));
        }

        private void HandleDistrictClick(object sender, District district)
        {
            if (lastOpenDistrict != district)
            {
                HideActiveDistrict();
            }
            lastOpenDistrict = district;

            if (lastOpenDistrict.MapMode != Mode)
                mapSettings.SetDistrictMaterials(lastOpenDistrict, Mode);
        }
        
        private void HandleDistrictDataLoaded(object sender, Dictionary<string, _District.Data> datas)
        {
            foreach (var key in districts.Keys)
            {
                districts[key].UpdateName(datas[key].name);
            }
        }

        private void HandleMapModeChanged(object sender, Mode mode)
        {
            if (lastOpenDistrict != null)
                mapSettings.SetDistrictMaterials(lastOpenDistrict, Mode);
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
            REE.Camera.Camera.Singleton.OnCameraZoomed += HandleCameraZoomed;

            District.OnClicked += HandleDistrictClick;
            Property.OnMouseOver += HandleMouseOverProperty;
            Property.OnMouseExited += HandleMouseExitedProperty;
            Property.OnSelected += HandlePropertySelected;
            Property.OnDeselected += HandlePropertyDeselected;

            OnMapModeChanged += HandleMapModeChanged;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();

            _District.DistrictManager.Instance.OnDistrictDataLoaded -= HandleDistrictDataLoaded;
            REE.Camera.Camera.Singleton.OnCameraZoomed -= HandleCameraZoomed;

            District.OnClicked -= HandleDistrictClick;
            Property.OnMouseOver -= HandleMouseOverProperty;
            Property.OnMouseExited -= HandleMouseExitedProperty;
            Property.OnSelected -= HandlePropertySelected;
            Property.OnDeselected -= HandlePropertyDeselected;

            OnMapModeChanged -= HandleMapModeChanged;
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
