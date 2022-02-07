using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Invesment
{
    [MoonSharpUserData]
    public class InvesmentDictionary
    {
        private static Dictionary<string, Texture2D> photos;
        public static Dictionary<string, Texture2D> Photos => photos;
        private static Dictionary<string, Dictionary<int, Invesment>> invesments;
        public static Dictionary<string, Dictionary<int, Invesment>> Invesments => invesments;

        public static Invesment GetInvesment(string tag, int id)
        {
            if (invesments.ContainsKey(tag) && id < invesments[tag].Count) return invesments[tag][id];
            return null;
        }

        public static Dictionary<int, Invesment> ConvertListOfInvesmentsToDictioanry(List<Invesment> invesments)
        {
            Dictionary<int, Invesment> dictionary = new Dictionary<int, Invesment>();

            foreach (Invesment invesment in invesments)
            {
                dictionary.Add(invesment.id, invesment);
            }

            return dictionary;
        }

        public static void SetInvesments(Dictionary<string, Dictionary<int, Invesment>> newInvesments)
        {
            if (invesments == null)
            {
                invesments = newInvesments;
                return;
            }

            foreach (var key in newInvesments.Keys)
            {
                if (invesments.ContainsKey(key))
                {
                    // If we already have a invesment type of this
                    // User might be trying to override
                    // So we should override everysingle one of them
                    foreach (var id in newInvesments[key].Keys)
                    {
                        // We are infact trying to override
                        if (invesments[key].ContainsKey(id))
                        {
                            // This should be always returning TRUE
                            if (invesments[key][id].id == newInvesments[key][id].id)
                            {
                                invesments[key][id] = newInvesments[key][id];
                            }
                        }
                        else // We are trying to add new invesments to this type
                        {
                            invesments[key].Add(id, newInvesments[key][id]);
                        }
                    }
                } else
                {
                    invesments.Add(key, newInvesments[key]);
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
