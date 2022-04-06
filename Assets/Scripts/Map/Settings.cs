using UnityEngine;
using System;
using System.Collections.Generic;
using Investment.Property;

namespace Map
{
    [CreateAssetMenu(fileName = "MapSettings", menuName = "ScriptableObjects/MapSettings", order = 0)]
    public class Settings : ScriptableObject
    {
        public Material SelectionMaterial;
        public Material HoverMaterial;

        public List<Material> OwnershipMaterials;

        public void UpdateBorderWidths(float zoom)
        {
            SelectionMaterial.SetFloat("_FirstOutlineWidth", Mathf.Max(0.6f, zoom * 5));
            HoverMaterial.SetFloat("_FirstOutlineWidth", Mathf.Max(0.6f, zoom * 5));
        }

        public void SetSelectionTexture(Texture texture)
        {
            SelectionMaterial.SetTexture("_MainTex", texture);
        }

        public void SetHoveringTexture(Texture texture)
        {
            HoverMaterial.SetTexture("_MainTex", texture);
        }

        public void SetSelectionColor(Color color)
        {
            SelectionMaterial.SetColor("_Color", color);
        }

        public void SetHoveringColor(Color color)
        {
            HoverMaterial.SetColor("_Color", color);
        }

        public void SetDistrictMaterials(District district, Mode mode)
        {
            district.MapMode = mode;
            foreach (var property in district.properties)
            {
                property.TrySetMaterial(GetMaterialForProperty(mode, property));
            }
        }

        public Material GetMaterialForProperty(Mode mode, Property property)
        {
            switch (mode)
            {
                case Mode.Texture:
                    return property.OriginalMaterial;
                case Mode.Ownership:
                    string company_tag = Company.CompanyManager.Instance.PlayerCompany.tag;

                    float ratio = 0;

                    Debug.LogError($"GetMaterialForProperty is NOT implemented!");
                    // if (Ownership.OwnershipDictionary.CompanyOwns(Investment.Type.Apartment, property.ID, out var ownership))
                    // {
                    //     Apartment apartment = ApartmentDictionary.Apartments[property.ID];
                    //     ratio = ownership.shares / investment.shares;
                    // }

                    return OwnershipMaterials[(int)ratio * 10];
            }
            return null;
        }
    }
}

