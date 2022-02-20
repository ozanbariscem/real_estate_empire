using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Investment
{
    [MoonSharpUserData]
    public class InvestmentDictionary
    {
        private static Dictionary<string, Texture2D> photos;
        public static Dictionary<string, Texture2D> Photos => photos;
        private static Dictionary<string, Dictionary<int, Investment>> investments;
        public static Dictionary<string, Dictionary<int, Investment>> Investments => investments;

        public static Investment GetInvestment(string tag, int id)
        {
            if (investments.TryGetValue(tag, out Dictionary<int, Investment> tags))
            {
                if (tags.TryGetValue(id, out Investment investment))
                    return investment;
            }
            return null;
        }

        public static Dictionary<int, Investment> ConvertListOfInvestmentsToDictioanry(List<Investment> investments)
        {
            Dictionary<int, Investment> dictionary = new Dictionary<int, Investment>();

            foreach (Investment investment in investments)
            {
                dictionary.Add(investment.id, investment);
            }

            return dictionary;
        }

        public static void SetInvestments(Dictionary<string, Dictionary<int, Investment>> newInvestments)
        {
            if (investments == null)
            {
                investments = newInvestments;
                return;
            }

            foreach (var key in newInvestments.Keys)
            {
                if (investments.ContainsKey(key))
                {
                    // If we already have a investment type of this
                    // User might be trying to override
                    // So we should override everysingle one of them
                    foreach (var id in newInvestments[key].Keys)
                    {
                        // We are infact trying to override
                        if (investments[key].ContainsKey(id))
                        {
                            // This should be always returning TRUE
                            if (investments[key][id].id == newInvestments[key][id].id)
                            {
                                investments[key][id] = newInvestments[key][id];
                            }
                        }
                        else // We are trying to add new invesments to this type
                        {
                            investments[key].Add(id, newInvestments[key][id]);
                        }
                    }
                } else
                {
                    investments.Add(key, newInvestments[key]);
                }
            }
        }

        public static void SetPhotos(Dictionary<string, Texture2D> newPhotos)
        {
            if (photos == null)
            {
                photos = newPhotos;
                return;
            }

            // If not null add on top of
            foreach (var key in newPhotos.Keys)
            {
                if (photos.ContainsKey(key))
                {
                    photos[key] = newPhotos[key];
                }
                else // Brand new photo
                {
                    photos.Add(key, newPhotos[key]);
                }
            }
        }
    }
}
