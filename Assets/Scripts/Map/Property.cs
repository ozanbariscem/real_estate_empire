using System;
using UnityEngine;

namespace Map
{
    public class Property : MonoBehaviour
    {
        public static event EventHandler<Property> OnMouseOver;
        public static event EventHandler<Property> OnMouseExited;
        public static event EventHandler<Property> OnSelected;
        public static event EventHandler<Property> OnDeselected;
        public static event EventHandler<Property> OnDoubleClicked;

        public int ID { get; private set; }
        public string District { get; private set; }

        public Texture Texture { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }
        public Material OriginalMaterial { get; private set; }

        public bool Selected { get; private set; }

        private float doubleClickRegisterTime = 0.2f;
        private float lastSelectionTime;

        public void TrySetMaterial(Material material)
        {
            MeshRenderer.sharedMaterial = material;
        }

        public void Set(int id, string district)
        {
            ID = id;
            District = district;

            MeshRenderer = GetComponent<MeshRenderer>();
            Texture = MeshRenderer.sharedMaterial.mainTexture;
            OriginalMaterial = MeshRenderer.sharedMaterial;
        }

        public void Outline(Material material)
        {
            MeshRenderer.sharedMaterial = material;
        }

        public void Deoutline(Material material)
        {
            MeshRenderer.sharedMaterial = material;
        }

        private void OnMouseEnter()
        {
            OnMouseOver?.Invoke(this, this);
        }

        private void OnMouseExit()
        {
            if (!Selected)
                OnMouseExited?.Invoke(this, this);
        }

        private void OnMouseDown()
        {
            if (!Selected)
                Select();
            else
                Deselect();
        }

        public void Select()
        {
            Selected = true;

            if (UnityEngine.Time.time <= lastSelectionTime + doubleClickRegisterTime)
            { 
                OnDoubleClicked?.Invoke(this, this);
            }
            lastSelectionTime = UnityEngine.Time.time;

            OnSelected?.Invoke(this, this);
        }

        public void Deselect()
        {
            Selected = false;
            OnDeselected?.Invoke(this, this);
        }
    }
}

