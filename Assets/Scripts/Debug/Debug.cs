using TMPro;
using System;
using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace REE.Debug
{
    [MoonSharpUserData]
    public class Debug : MonoBehaviour
    {
        public static Debug Instance { get; private set; }
        public static event EventHandler<State> OnToggled;

        public static State State { get; private set; }

        private TextMeshProUGUI headerText;
        private TextMeshProUGUI listText;
        private Transform listContent;
        private Transform listTransform;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Start()
        {
            State = new State();

            listTransform = transform.Find("GameMenu/DebugMenu/List");
            listContent = listTransform.Find("Scroll View/Viewport/Content");
            listText = listContent.Find("StringPrefab").gameObject.GetComponent<TextMeshProUGUI>();
            headerText = listTransform.Find("Header").gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void Set(bool active)
        {
            State.IsActive = active;
            OnToggled?.Invoke(this, State);
        }

        public void ListInvestments(string type, int page)
        {
            if (!State.IsActive) return;

            List<Investment.Investment> investments = 
                Investment.InvestmentDictionary.Investments[type].Values.
                Where(x => x.id >= page * 100 && x.id < 100 + page*100).ToList();

            List(investments);
        }

        public void List<T>(List<T> list) where T : class
        {
            if (!State.IsActive) return;

            headerText.text = $"{typeof(T)}";
            listText.text = "";

            int i;
            for (i = 0; i < list.Count; i++)
            {
                listText.text += $"\n{list[i]}";
            }

            listText.rectTransform.sizeDelta = 
                new Vector2(listText.rectTransform.sizeDelta.x, listText.preferredHeight);
            listText.ForceMeshUpdate();
            listTransform.gameObject.SetActive(true);
        }
    }
}
