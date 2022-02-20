using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Modifier
{
    [MoonSharpUserData]
    public class ModifierManager : Manager.Manager
    {
        public static ModifierManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, ModifierData>> OnModifiersLoaded;

        public event EventHandler<Modifier> OnModifierAdded;
        public event EventHandler<Modifier> OnModifierRemoved;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int i = 0;
                foreach (var investment in Investment.InvestmentDictionary.Investments["property"].Values.ToList())
                {
                    if (i++ == 1)
                        break;
                    AddModifier(
                        "property", investment.id, "homeless",
                        new Time.Date(0, (ushort)(1) , 0, 0));
                }
            }
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "modifiers/manager.lua";

            LoadScript();
            LoadModifiers();

            RaiseOnRulesLoaded();
        }

        public void AddModifier(string type, int id, string modifier_tag, Time.Date _date=null)
        {
            Time.Date currentDate = Time.TimeManager.Instance.Date;
            Time.Date date = _date;
            if (_date == null)
                date = ModifierDictionary.Dictionary[modifier_tag].effective;

            date = new Time.Date(
                (ushort)(currentDate.hour + date.hour),
                (ushort)(currentDate.day + date.day),
                (ushort)(currentDate.month + date.month),
                (ushort)(currentDate.year + date.year));

            Modifier modifier = new Modifier(type, id, modifier_tag, date);
            ModifierDictionary.AddModifier(type, id, modifier);
            OnModifierAdded?.Invoke(this, modifier);
        }

        public void RemoveModifier(string type, int id, string modifier_tag)
        {
            Modifier modifier = ModifierDictionary.RemoveModifier(type, id, modifier_tag);
            OnModifierRemoved?.Invoke(this, modifier);
        }

        private void HandleModifierAdded(object sender, Modifier modifier)
        {
            Investment.Investment investment = Investment.InvestmentDictionary.GetInvestment(modifier.type, modifier.investment_id);
            investment.CalculateValue();
        }

        private void HandleModifierRemoved(object sender, Modifier modifier)
        {
            Investment.Investment investment = Investment.InvestmentDictionary.GetInvestment(modifier.type, modifier.investment_id);
            investment.CalculateValue();
        }

        private void HandleDayPass(object sender, Time.Date date)
        {
            int i;
            for (i = 0; i < ModifierDictionary.ModifiersSorted.Count; i++)
            {
                Modifier modifier = ModifierDictionary.ModifiersSorted[i];
                if (modifier.endDate.CompareTo(date) > 0)
                {
                    break;
                }
            }
            
            for (int j = 0; j < i; j++)
            {
                Modifier modifier = ModifierDictionary.ModifiersSorted[0];
                ModifierDictionary.ModifiersSorted.RemoveAt(0);
                RemoveModifier(modifier.type, modifier.investment_id, modifier.modifier_data_tag);
            }
        }

        #region CONTENT LOADER
        private void LoadModifiers()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/modifiers/modifiers.json");
            if (json == null) return;

            ModifierDictionary.LoadModifierDatas(JsonConvert.DeserializeObject<List<ModifierData>>(json));
            OnModifiersLoaded?.Invoke(this, ModifierDictionary.Dictionary);
        }
        #endregion

        #region SUBSCRIPTIONS
        protected override void Subscribe()
        {
            base.Subscribe();
            Time.TimeManager.Instance.OnDayPass += HandleDayPass;

            OnModifierAdded += HandleModifierAdded;
            OnModifierRemoved += HandleModifierRemoved;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Time.TimeManager.Instance.OnDayPass -= HandleDayPass;

            OnModifierAdded -= HandleModifierAdded;
            OnModifierRemoved -= HandleModifierRemoved;
        }
        #endregion
    }
}

