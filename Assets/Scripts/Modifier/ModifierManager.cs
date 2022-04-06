using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using Investment;
using UnityEngine;

namespace Modifier
{
    [MoonSharpUserData]
    public class ModifierManager : Manager.Manager
    {
        public static ModifierManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, ModifierData>> OnModifierDatasLoaded;
        public event EventHandler<Dictionary<string, ModifierData>> OnModifiersLoaded;

        public event EventHandler<Modifier> OnModifierAdded;
        public event EventHandler<Modifier> OnModifierRemoved;

        private Time.Date date;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "modifiers/manager.lua";

            LoadScript();
            LoadModifiers();

            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadActiveModifiers(path);
            RaiseOnContentLoaded();
        }

        public void AddGlobalModifier(string type, string modifier_tag,
            ushort hour = 0, ushort day = 0, ushort month = 0, ushort year = 0)
        {
            Time.Date date = null;
            if (!(hour == day && day == month && month == year && year == 0))
                date = new Time.Date(hour, day, month, year);
            AddGlobalModifier(type, modifier_tag, date);
        }

        public void AddGlobalModifier(string type, string modifier_tag, Time.Date _date = null)
        {
            Console.Console.Run("log_error HELLO ADD GLOBAL MODIFIER IS NOT IMPLEMENTED!!");
            Debug.LogError("HELLO ADD GLOBAL MODIFIER IS NOT IMPLEMENTED!!");
            // foreach (var id in InvestmentDictionary.Investments[type].Keys)
            // {
            //     AddModifier(type, id, modifier_tag, _date);
            // }
        }

        public void AddModifier(string type, int id, string modifier_tag,
            ushort hour = 0, ushort day = 0, ushort month = 0, ushort year = 0)
        {
            Time.Date date = null;
            if (!(hour == day && day == month && month == year && year == 0))
            {
                date = new Time.Date(hour, day, month, year);
            }
            AddModifier(type, id, modifier_tag, date);
        }

        public void AddModifier(string type, int id, string modifier_tag, Time.Date _date=null)
        {
            Time.Date date = _date;
            if (_date == null)
                date = ModifierDictionary.Dictionary[modifier_tag].effective;

            date = new Time.Date(
                (ushort)(this.date.hour + date.hour),
                (ushort)(this.date.day + date.day),
                (ushort)(this.date.month + date.month),
                (ushort)(this.date.year + date.year));

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
            Console.Console.Run("log_error HELLO HandleModifierAdded IS NOT IMPLEMENTED!!");
            Debug.LogError("HELLO HandleModifierAdded IS NOT IMPLEMENTED!!");
            //Investment.Investment investment = InvestmentDictionary.GetInvestment(modifier.investment_type, modifier.investment_id);
            //investment.CalculateValue();
        }

        private void HandleModifierRemoved(object sender, Modifier modifier)
        {
            // There is a chance an overriden modifier was already removed and handled
            // But not removed from the sorted list for performance reasons
            // in that case we could just skip because the value is already recalculated for new values
            if (modifier == null) return;

            Console.Console.Run("log_error HELLO HandleModifierRemoved IS NOT IMPLEMENTED!!");
            Debug.LogError("HELLO HandleModifierRemoved IS NOT IMPLEMENTED!!");
            //Investment.Investment investment = InvestmentDictionary.GetInvestment(modifier.investment_type, modifier.investment_id);
            //investment.CalculateValue();
        }

        private void HandleDayPass(object sender, Time.Date date)
        {
            if (ModifierDictionary.ModifiersSorted == null || ModifierDictionary.ModifiersSorted.Count == 0) return;

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

                RemoveModifier(modifier.investment_type, modifier.investment_id, modifier.modifier_data_tag);
            }
        }

        private void HandleStartDateLoaded(object sender, Time.Date date)
        {
            this.date = date;
        }

        #region CONTENT LOADER
        private void LoadModifiers()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/modifiers/modifiers.json");
            if (json == null) return;

            ModifierDictionary.LoadModifierDatas(JsonConvert.DeserializeObject<List<ModifierData>>(json));
            OnModifierDatasLoaded?.Invoke(this, ModifierDictionary.Dictionary);
        }

        private void LoadActiveModifiers(string path)
        {
            string json = Utils.ContentHandler.SafeGetString($"{path}/modifiers/modifiers.json");
            if (json == null) return;

            ModifierDictionary.LoadModifiers(JsonConvert.DeserializeObject<
                Dictionary<string, Dictionary<int, Dictionary<string, Modifier>>>>(json));
            OnModifiersLoaded?.Invoke(this, ModifierDictionary.Dictionary);
        }
        #endregion

        #region SUBSCRIPTIONS
        protected override void Subscribe()
        {
            base.Subscribe();
            Time.TimeManager.Instance.OnStartDateLoaded += HandleStartDateLoaded;
            Time.TimeManager.Instance.OnDayPass += HandleDayPass;

            OnModifierAdded += HandleModifierAdded;
            OnModifierRemoved += HandleModifierRemoved;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Time.TimeManager.Instance.OnStartDateLoaded -= HandleStartDateLoaded;
            Time.TimeManager.Instance.OnDayPass -= HandleDayPass;

            OnModifierAdded -= HandleModifierAdded;
            OnModifierRemoved -= HandleModifierRemoved;
        }
        #endregion
    }
}