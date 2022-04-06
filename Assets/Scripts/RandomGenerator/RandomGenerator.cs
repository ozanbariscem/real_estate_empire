using System.Collections.Generic;
using Investment.Property;
using Newtonsoft.Json;
using UnityEngine;
using District;

[ExecuteAlways]
public class RandomGenerator : MonoBehaviour
{
    [TextArea]
    public string note = "Don't forget to feed RNG for each session.\n If you want it to be consistent. Call every function in one with FeedRng() first.";

    public List<string> buildingNames;

    public int seed = -1;
    private System.Random rng;

    public void FeedRng()
    {
        if (seed == -1)
            rng = new System.Random();
        else
            rng = new System.Random(seed);
    }

    public void GenerateRandomBuildings()
    {
        FeedRng();

        string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/district/districts.json");
        if (json == null) return;

        List<Data> datas = JsonConvert.DeserializeObject<List<Data>>(json);
        List<Building> buildings = new List<Building>();
        List<Apartment> apartments = new List<Apartment>();

        int building_id = 0;
        int apartment_id = 0;
        foreach (var data in datas)
        {
            for (int c = 0; c < data.size; c++)
            {
                Building building = new Building();
                building.id = building_id++;
                building.district_tag = data.tag;

                // -- RANDOM VARIABLES --
                building.name = buildingNames[rng.Next(buildingNames.Count-1)].TrimEnd(' ');
                building.age = 2012 - GetRandomYear();
                building.floor_count = GetRandomFloorCountForAge(building.age);
                building.floor_size = GetRandomFloorSizeForAge(building.age);
                building.apartments = new List<int>();

                for (int floor = 0; floor < building.floor_count; floor++)
                {
                    for (int door = 0; door < building.floor_size; door++)
                    {
                        // -- APARTMENT --
                        Apartment apartment = new Apartment();
                        apartment.id = apartment_id++;
                        apartment.building_id = building.id;
                        apartment.floor_index = floor;
                        apartment.door_index = door;

                        apartment.price = 60000;

                        apartments.Add(apartment);
                        building.apartments.Add(apartment.id);
                    }
                }
                buildings.Add(building);
            }
        }

        // SAVE BUILDINGS
        json = JsonConvert.SerializeObject(buildings, Formatting.Indented);
        Utils.StreamingAssetsHandler.SafeSetString("vanilla/investment/property/buildings.json", json);

        // SAVE APARTMENTS
        json = JsonConvert.SerializeObject(apartments, Formatting.Indented);
        Utils.StreamingAssetsHandler.SafeSetString("vanilla/investment/property/apartments.json", json);
    }

    private int GetRandomYear()
    {
        int year = 0;
        int number = rng.Next(100);
        
        if (number < 8)
        {
            year = 1890;
        } else if (number < 8+16)
        {
            year = 1900;
        } else if (number < 8+16+14)
        {
            year = 1910;
        } else if (number < 8 + 16 + 14 + 21)
        {
            year = 1920;
        } else if (number < 8 + 16 + 14 + 21 + 13)
        {
            year = 1930;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2)
        {
            year = 1940;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2 + 4)
        {
            year = 1950;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2 + 4 + 5)
        {
            year = 1960;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2 + 4 + 5 + 3)
        {
            year = 1970;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2 + 4 + 5 + 3 + 3)
        {
            year = 1980;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2 + 4 + 5 + 3 + 3 + 3)
        {
            year = 1990;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2 + 4 + 5 + 3 + 3 + 3 + 7)
        {
            year = 2000;
        } else if (number < 8 + 16 + 14 + 21 + 13 + 2 + 4 + 5 + 3 + 3 + 3 + 7 + 1)
        {
            year = 2010;
        }

        return year + rng.Next(10);
    }

    private int GetRandomFloorCountForAge(int age)
    {
        if (age > 50)
        {
            return 4 + rng.Next(5);
        } else if (age > 10)
        {
            return 9 + rng.Next(5);
        }
        else
        {
            return 15 + rng.Next(5);
        }
    }

    private int GetRandomFloorSizeForAge(int age)
    {
        return 8 + rng.Next(5);
    }
}
