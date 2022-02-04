using System;
using System.Collections.Generic;

namespace Language
{
    [Serializable]
    public class Dictionary
    {
        private Dictionary<string, string> translations;

        public Dictionary(Dictionary<string, string> translations)
        {
            this.translations = translations;
        }

        public string Get(string tag)
        {
            if (translations.ContainsKey(tag)) 
                return translations[tag];
            return tag;
        }
    }
}

