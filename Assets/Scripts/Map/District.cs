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
        public enum State { CLOSING, CLOSED, OPENING, OPEN }

        public event EventHandler<District> OnClicked;
        public event EventHandler<District> OnDoubleClicked;
        public event EventHandler<Property> OnMouseOverProperty;
        public event EventHandler<Property> OnPropertySelected;
        public event EventHandler<Property> OnPropertyDeselected;

        public string district_tag;

        public Vector3 Center => hitboxes[0].position;

        private LineRenderer border;
        private Transform canvas;

        private Transform LOD0;
        private Transform LOD1;
        private List<Transform> hitboxes;

        private TextMeshProUGUI nameText;

        private float zoom;
        private float transitionSpeed = 2f;

        private float doubleClickRegisterTime;
        private float lastClickTime;

        public Material borderMaterial;

        private void Start()
        {
            transitionSpeed = 5f;
            doubleClickRegisterTime = 0.2f;

            district_tag = name;

            Subscribe();
            GetComponents();

            Chess.Camera.CameraController.Singleton.OnCameraZoomed += HandleCameraZoomed;
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

            Zoom();
        }

        private void Zoom()
        {
            if (Mathf.Abs(LOD1.localScale.y - zoom) > .01f)
            {
                float zoom = this.zoom;
                if (zoom > .95f)
                    zoom = 1f;
                LOD1.localScale = Vector3.Lerp(LOD1.localScale, new Vector3(1, Mathf.Max(0.01f, 1-zoom), 1), UnityEngine.Time.deltaTime * transitionSpeed);
            }
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
                        Debug.Log("Im a buildinggg HANDLE MEEEEEE");
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

        private void GetComponents()
        {
            border = transform.Find("Border").GetComponent<LineRenderer>();
            border.sortingOrder = 99;

            canvas = transform.Find("Canvas");
            nameText = canvas.Find("Text").GetComponent<TextMeshProUGUI>();

            borderMaterial = border.material;
            borderMaterial.SetFloat("_MinPulseVal", 1f);

            hitboxes = new List<Transform>();

            LOD0 = transform.Find("Buildings/LOD0");
            LOD1 = transform.Find("Buildings/LOD1");
            foreach (Transform hitbox in transform.Find("Hitbox"))
            {
                hitboxes.Add(hitbox);
            }

            foreach (Transform building in LOD0)
            {
                if (int.TryParse(building.name, out int id))
                {
                    Property property = building.gameObject.AddComponent<Property>();
                    property.Set(id, district_tag);
                    property.OnMouseOverProperty += HandleMouseOverProperty;
                    property.OnPropertySelected += HandlePropertySelected;
                    property.OnPropertyDeselected += HandlePropertyDeselected;
                }
            }
        }

        public void UpdateName(string name)
        {
            nameText.text = name;
        }

        private void HandleCameraZoomed(object sender, float zoom)
        {
            this.zoom = zoom;

            border.startWidth = Mathf.Max(.2f, zoom);
            border.endWidth = Mathf.Max(.2f, zoom);
            
            Vector3 borderPosition = border.transform.position;
            borderPosition.y = Mathf.Max(hitboxes[0].position.y, zoom / 2f);
            border.transform.position = borderPosition;

            if (zoom > .5f)
                zoom = 1;
            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, zoom);
            nameText.gameObject.SetActive(zoom > .3f);
        }

        private void HandleMouseOverProperty(object sender, Property property)
        {
            OnMouseOverProperty?.Invoke(sender, property);
        }

        private void HandlePropertySelected(object sender, Property property)
        {
            OnPropertySelected?.Invoke(sender, property);
        }

        private void HandlePropertyDeselected(object sender, Property property)
        {
            OnPropertyDeselected?.Invoke(sender, property);
        }

        private void HandleOnClicked(object sender, District district)
        {
            if (UnityEngine.Time.time - lastClickTime <= doubleClickRegisterTime)
            { 
                OnDoubleClicked?.Invoke(this, district);
            }

            lastClickTime = UnityEngine.Time.time;
            Show();
        }

        private void HandleOnDoubleClicked(object sender, District district)
        {
            
        }

        public void Show()
        {
            LOD0.gameObject.SetActive(true);
            LOD1.gameObject.SetActive(false);

            nameText.gameObject.SetActive(false);

            foreach (Transform hitbox in hitboxes)
            {
                hitbox.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }

            borderMaterial.SetFloat("_MinPulseVal", .5f);
        }

        public void Hide()
        {
            LOD0.gameObject.SetActive(false);
            LOD1.gameObject.SetActive(true);

            nameText.gameObject.SetActive(true);

            foreach (Transform hitbox in hitboxes)
            {
                hitbox.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }

            borderMaterial.SetFloat("_MinPulseVal", 1f);
        }

        private void Subscribe()
        {
            OnClicked += HandleOnClicked;
            OnDoubleClicked += HandleOnDoubleClicked;
        }

        private void Unsubscribe()
        {
            OnClicked -= HandleOnClicked;
            OnDoubleClicked -= HandleOnDoubleClicked;
        }
    }
}


