using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Invesment
{
    [MoonSharpUserData]
    public class InvesmentDictionary
    {
        private static Dictionary<string, Texture2D> photos;
        private static Dictionary<string, List<Invesment>> invesments;

        public InvesmentDictionary() { }

        public InvesmentDictionary(Dictionary<string, List<Invesment>> dictionary)
        {
            invesments = dictionary;

            photos = new Dictionary<string, Texture2D>();
            foreach (var key in invesments.Keys)
            {
                foreach (var invesment in invesments[key])
                {
                    if (!photos.ContainsKey(invesment.photo))
                    {
                        Texture2D texture = Utils.StreamingAssetsHandler.SafeGetTexture($"vanilla/invesment/{key}/photos/{invesment.photo}");
                        photos.Add(invesment.photo, texture);
                    }
                    invesment.texture = photos[invesment.photo];
                }
            }
        }

        public static Invesment GetInvesment(string tag, int id)
        {
            if (invesments.ContainsKey(tag) && id < invesments[tag].Count) return invesments[tag][id];
            return null;
        }
    }
}
