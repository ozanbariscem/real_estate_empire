using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using Newtonsoft.Json;

namespace SaveFile
{
    [MoonSharpUserData]
    public class SaveFileManager : Manager.Manager
    {
        public static SaveFileManager Instance { get; private set; }

        public event EventHandler OnSaveFileDirectoryCreated;
        public event EventHandler OnCurrentGameSaved;
        public event EventHandler<List<Data>> OnScenarioDatasLoaded;
        public event EventHandler<List<Data>> OnSaveFileDatasLoaded;

        public event EventHandler<Data> OnDataFileLoadRequested;

        private string saveDirectoryPath;

        public List<Data> scenarioFiles;
        public List<Data> saveFiles;

        private string currentSaveFileName;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDirectoryPath = Path.Combine(documentsPath, "Lucy Software/Real Estate Empire/save games");

            scriptPath = "save_file/manager.lua";

            LoadScript();
            CheckAndCreateSaveFolder();

            GetScenarioFiles();
            GetSaveFiles();

            RaiseOnRulesLoaded();
        }

        /// <summary>
        /// Checks if the Save Folder exists in Documents path, if not creates one
        /// </summary>
        private void CheckAndCreateSaveFolder()
        {
            if (!File.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
                OnSaveFileDirectoryCreated?.Invoke(this, EventArgs.Empty);
            }
        }

        private List<Data> GetDataFiles(string path)
        {
            List<Data> list = new List<Data>();
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                string status = GetSaveFileStatus(directory);
                Data data = Data.FromFileInfo(new FileInfo(directory), status);
                list.Add(data);
            }
            return list.OrderBy(x => x.lastWriteTime.TimeOfDay).ToList();
        }

        private void GetScenarioFiles()
        {
            scenarioFiles = GetDataFiles(Path.Combine(Application.streamingAssetsPath, "vanilla/scenarios"));
            OnScenarioDatasLoaded?.Invoke(this, scenarioFiles);
        }

        private void GetSaveFiles()
        {
            saveFiles = GetDataFiles(saveDirectoryPath);
            OnSaveFileDatasLoaded?.Invoke(this, saveFiles);
        }

        private string GetSaveFileStatus(string path)
        {
            if (!File.Exists(Path.Combine(path, "calendar/start_date.txt"))) return "CORRUPTED";
            if (!File.Exists(Path.Combine(path, "ownership/ownerships.json"))) return "CORRUPTED";
            if (!File.Exists(Path.Combine(path, "loan/loans.json"))) return "CORRUPTED";
            if (!File.Exists(Path.Combine(path, "district/districts.json"))) return "CORRUPTED";
            if (!Directory.Exists(Path.Combine(path, "invesment"))) return "CORRUPTED";

            foreach (var type in Invesment.Types.Dictionary.Keys)
            {
                if (!Directory.Exists($"{path}/invesment/{type}")) return "CORRUPTED";
            }

            return "OKAY";
        }

        private void HandleDayPass(object sender, Time.Date date)
        {
            SaveCurrentFile(date);
        }

        private void SaveCurrentFile(Time.Date date)
        {
            string path = Path.Combine(saveDirectoryPath, currentSaveFileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + "/calendar");
                Directory.CreateDirectory(path + "/ownership");
                Directory.CreateDirectory(path + "/invesment");
                Directory.CreateDirectory(path + "/loan");
                Directory.CreateDirectory(path + "/district");
                foreach (var type in Invesment.Types.Dictionary.Keys)
                {
                    Directory.CreateDirectory($"{path}/invesment/{type}");
                }
            } 
            // Save start date
            Utils.ContentHandler.SafeSetString($"{path}/calendar/start_date.txt", JsonConvert.SerializeObject(date));
            // Save ownerships
            Utils.ContentHandler.SafeSetString($"{path}/ownership/ownerships.json", JsonConvert.SerializeObject(Ownership.OwnershipList.List));
            // Save loans
            Utils.ContentHandler.SafeSetString($"{path}/loan/loans.json", JsonConvert.SerializeObject(Loan.LoanList.Loans.Values.ToList()));
            // Save districts
            Utils.ContentHandler.SafeSetString($"{path}/district/districts.json", JsonConvert.SerializeObject(District.DistrictDictionary.Dictionary.Values.ToList()));
            // Save invesments
            foreach (var type in Invesment.Types.Dictionary.Keys)
            {
                Utils.ContentHandler.SafeSetString(
                $"{path}/invesment/{type}/invesments.json",
                JsonConvert.SerializeObject(Invesment.InvesmentDictionary.Invesments[type].Values.ToList()));
            }

            OnCurrentGameSaved?.Invoke(this, EventArgs.Empty);
        }

        public void LoadSaveFile(Data data)
        {
            if (data.Status == "OKAY")
            {
                OnDataFileLoadRequested?.Invoke(this, data);
            }
        }

        public bool SaveFileAlreadyExists(string name)
        {
            return Directory.Exists(Path.Combine(saveDirectoryPath, name));
        }

        public void SetCurrentSaveFileName(string name)
        {
            currentSaveFileName = name;
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            Time.TimeManager.Instance.OnDayPass += HandleDayPass;
        }

        protected override void Unsubscribe()
        {
            base.Subscribe();
            Time.TimeManager.Instance.OnDayPass -= HandleDayPass;
        }
    }
}

