using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace District
{
    [MoonSharpUserData]
    public class DistrictDictionary
    {
        public static Dictionary<string, District> Dictionary;

        public static District SafeGet(string tag)
        {
            Dictionary.TryGetValue(tag, out District district);
            return district;
        }

        public static Dictionary<string, District> LoadJson(string json)
        {
            List<District> newDistricts = JsonConvert.DeserializeObject<List<District>>(json);

            if (Dictionary == null)
                Dictionary = new Dictionary<string, District>();

            foreach (District district in newDistricts)
            {
                if (Dictionary.TryGetValue(district.tag, out District oldDistrict))
                {
                    oldDistrict = district;
                }
                else
                {
                    Dictionary.Add(district.tag, district);
                }
            }

            return Dictionary;
        }
    }
}
