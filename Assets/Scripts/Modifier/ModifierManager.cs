using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;

namespace Modifier
{
    [MoonSharpUserData]
    public class ModifierManager : Manager.Manager
    {
        public static ModifierManager Instance { get; private set; }

        public event EventHandler<Dictionary<string, Group>> OnGroupsLoaded;
        public event EventHandler<Dictionary<string, Modifier>> OnModifiersLoaded;

        public event EventHandler<
            Dictionary<string, Dictionary<string, Dictionary<string, bool>>>> OnActiveModifiersLoaded;

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
            LoadGroups();
            LoadModifiers();

            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadActiveModifiers(path);

            RaiseOnContentLoaded();
        }

        private void LoadGroups()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/modifiers/groups.json");
            if (json == null) return;

            Group.LoadGroups(JsonConvert.DeserializeObject<List<Group>>(json));
            OnGroupsLoaded?.Invoke(this, Group.Groups);
        }

        private void LoadModifiers()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/modifiers/modifiers.json");
            if (json == null) return;

            ModifierDictionary.LoadModifiers(JsonConvert.DeserializeObject<List<Modifier>>(json));
            OnModifiersLoaded?.Invoke(this, ModifierDictionary.Modifiers);
        }

        private void LoadActiveModifiers(string path)
        {
            if (Directory.Exists($"{path}/modifiers/active_modifiers"))
            {
                DirectoryInfo directory = new DirectoryInfo($"{path}/modifiers/active_modifiers");

                foreach (var file in directory.GetFiles())
                {
                    if (file.Extension == ".json")
                    {
                        string json = Utils.ContentHandler.SafeGetString($"{file}");
                        if (json == null) continue;

                        Dictionary<string, Dictionary<string, bool>> modifiers = 
                            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(json);

                        ModifierDictionary.LoadActiveModifiers(Path.GetFileNameWithoutExtension(file.FullName), modifiers);
                    }
                }
            }

            OnActiveModifiersLoaded?.Invoke(this, ModifierDictionary.ActiveModifiers);
        }
    }
}

