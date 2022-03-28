using TMPro;
using System;
using UnityEngine;
using MoonSharp.Interpreter;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Map 
{
    [MoonSharpUserData]
    public class District : MonoBehaviour
    {
        public static event EventHandler<District> OnClicked;
        public static event EventHandler<District> OnDoubleClicked;

        public string district_tag;

        public Vector3 Center => hitboxes[0].position;
        public Mode MapMode;

        public List<Property> properties;

        private LineRenderer border;
        private Transform canvas;

        private Transform[] roadLODs;
        private Transform[] buildingLODs;

        private Transform hitbox;
        private List<Transform> hitboxes;

        private TextMeshProUGUI nameText;

        private float doubleClickRegisterTime = 0.2f;
        private float lastClickTime;

        private Material borderMaterial;

        private void Start()
        {
            district_tag = name;

            Subscribe();
            SetComponents();
            SetProperties();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Update()
        {
            if (IsClicked())
            {
                OnClicked?.Invoke(this, this);
            }
        }

        public void Show() => Toggle(true);

        public void Hide() => Toggle(false);

        public void Toggle(bool visible)
        {
            buildingLODs[0].gameObject.SetActive(visible);
            buildingLODs[1].gameObject.SetActive(!visible);

            nameText.gameObject.SetActive(!visible);
            hitbox.gameObject.SetActive(!visible);

            borderMaterial.SetFloat("_MinPulseVal", visible ? .5f : 1f);
        }

        public void UpdateName(string name)
        {
            nameText.text = name;
        }

        private bool IsClicked()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return false;

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000f);
                
                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag("BUILDING"))
                    {
                        return hit.transform.parent.parent.parent == transform;
                    }

                    foreach (Transform hitbox in hitboxes)
                    {
                        if (hit.transform == hitbox)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #region HANDLERS
        private void HandleCameraZoomed(object sender, float zoom)
        {
            if (zoom > .5f)
            {
                roadLODs[0].gameObject.SetActive(false);
                roadLODs[1].gameObject.SetActive(true);
            } else
            {
                Vector3 cameraPosition = REE.Camera.Camera.Singleton.cameraTransform.position;
                if (Vector3.Distance(cameraPosition, Center) < 300f)
                {
                    roadLODs[0].gameObject.SetActive(true);
                    roadLODs[1].gameObject.SetActive(false);
                } else
                {
                    roadLODs[0].gameObject.SetActive(false);
                    roadLODs[1].gameObject.SetActive(true);
                }
            }

            // border.startWidth = Mathf.Max(.2f, zoom);
            // border.endWidth = Mathf.Max(.2f, zoom);
            // 
            // Vector3 borderPosition = border.transform.position;
            // borderPosition.y = Mathf.Max(hitboxes[0].position.y, zoom / 2f);
            // border.transform.position = borderPosition;

            if (zoom > .5f)
                zoom = 1;
            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, zoom);
            nameText.gameObject.SetActive(zoom > .3f);
        }

        private void HandleOnClicked(object sender, District district)
        {
            if (district != this) return;

            if (UnityEngine.Time.time - lastClickTime <= doubleClickRegisterTime)
            { 
                OnDoubleClicked?.Invoke(this, district);
            }

            lastClickTime = UnityEngine.Time.time;
            Show();
        }

        private void HandleOnDoubleClicked(object sender, District district)
        {
            if (district != this) return;
        }
        #endregion

        #region SETUP
        private void SetComponents()
        {
            SetBorder();
            SetUI();
            SetHitboxes();
            SetLODs();
        }
        
        private LineRenderer SetBorder()
        {
            border = transform.Find("Border").GetComponent<LineRenderer>();
            border.sortingOrder = 99;

            borderMaterial = border.sharedMaterial;
            borderMaterial.SetFloat("_MinPulseVal", 1f);

            return border;
        }

        private Transform SetUI()
        {
            canvas = transform.Find("Canvas");
            nameText = canvas.Find("Text").GetComponent<TextMeshProUGUI>();
            return canvas;
        }
        
        private List<Transform> SetHitboxes()
        {
            hitbox = transform.Find("Hitbox");
            hitboxes = new List<Transform>();
            foreach (Transform hitbox in hitbox)
            {
                hitboxes.Add(hitbox);
            }
            return hitboxes;
        }
        
        private void SetLODs()
        {
            roadLODs = new Transform[2];
            roadLODs[0] = transform.Find("Roads/LOD0");
            roadLODs[1] = transform.Find("Roads/LOD1");

            buildingLODs = new Transform[2];
            buildingLODs[0] = transform.Find("Buildings/LOD0");
            buildingLODs[1] = transform.Find("Buildings/LOD1");
        }
        
        private void SetProperties()
        {
            if (buildingLODs == null || buildingLODs.Length < 1)
            {
                Debug.LogError($"Couldn't set properties in district {district_tag}!");
                return;
            }

            properties = new List<Property>();
            foreach (Transform building in buildingLODs[0])
            {
                if (int.TryParse(building.name, out int id))
                {
                    Property property = building.gameObject.AddComponent<Property>();
                    property.Set(id, district_tag);
                    properties.Add(property);
                }
            }
        }
        #endregion

        #region SUBSCRIPTION
        private void Subscribe()
        {
            OnClicked += HandleOnClicked;
            OnDoubleClicked += HandleOnDoubleClicked;
            REE.Camera.Camera.Singleton.OnCameraZoomed += HandleCameraZoomed;
        }

        private void Unsubscribe()
        {
            OnClicked -= HandleOnClicked;
            OnDoubleClicked -= HandleOnDoubleClicked;
            REE.Camera.Camera.Singleton.OnCameraZoomed -= HandleCameraZoomed;
        }
        #endregion
    }
}


