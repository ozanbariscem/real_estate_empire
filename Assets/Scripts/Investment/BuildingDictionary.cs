using System;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Investment.Property
{
    [MoonSharpUserData]
    public class BuildingDictionary
    {
        public static event EventHandler<Building> OnBuildingAdded;
        public static event EventHandler<List<Building>> OnBuildingsAdded;

        public static Dictionary<string, Dictionary<int, Building>> Buildings { get; private set; }
        
        public static void LoadBuildings(List<Building> buildings)
        {
            foreach (var building in buildings)
                AddBuilding(building);
            OnBuildingsAdded?.Invoke(null, buildings);
        }

        public static void AddBuilding(Building building)
        {
            if (Buildings == null)
                Buildings = new Dictionary<string, Dictionary<int, Building>>();

            if (Buildings.TryGetValue(building.district_tag, out var district))
            {
                if (district.TryGetValue(building.id, out var _building))
                    _building = building;
                else
                    district.Add(building.id, building);
                OnBuildingAdded?.Invoke(null, building);
            }
        }
    }
}

