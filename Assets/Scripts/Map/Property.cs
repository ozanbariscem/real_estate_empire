using System;
using UnityEngine;

namespace Map
{
    public class Property : MonoBehaviour
    {
        public event EventHandler<Property> OnMouseOverProperty;
        public event EventHandler<Property> OnPropertySelected;
        public event EventHandler<Property> OnPropertyDeselected;

        public int ID { get; private set; }
        public string District { get; private set; }

        public Texture Texture { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }
        public Material Material { get; private set; }

        public bool Selected { get; private set; }

        public void Set(int id, string district)
        {
            ID = id;
            District = district;

            MeshRenderer = GetComponent<MeshRenderer>();
            Texture = MeshRenderer.material.mainTexture;
            Material = MeshRenderer.sharedMaterial;
        }

        public void Deoutline()
        {
            MeshRenderer.sharedMaterial = Material;
        }

        public void Outline(Material material)
        {
            MeshRenderer.sharedMaterial = material;
        }

        private void OnMouseEnter()
        {
            OnMouseOverProperty?.Invoke(this, this);
        }

        private void OnMouseExit()
        {
            if (!Selected)
                Deoutline();
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
            OnPropertySelected?.Invoke(this, this);
        }

        public void Deselect()
        {
            Selected = false;
            OnPropertyDeselected?.Invoke(this, this);
        }
    }
}

