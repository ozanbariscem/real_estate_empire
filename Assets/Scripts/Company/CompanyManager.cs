using System;
using UnityEngine;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Company
{
    [MoonSharpUserData]
    public class CompanyManager : Manager.Manager
    {
        public static CompanyManager Instance { get; private set; }

        public event EventHandler<Company> OnPlayerCompanyLoaded;
        public event EventHandler<List<Company>> OnCompaniesLoaded;

        public event EventHandler<Company> OnPlayerCompanyChanged;

        public Company PlayerCompany { get; private set; }

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        public void ChangeCompany(string tag)
        {
            if (PlayerCompany != null)
            {
                if (PlayerCompany.tag == tag) return;

                if (CompanyDictionary.Dictionary.TryGetValue(tag, out var company))
                {
                    PlayerCompany.isAI = true;
                    PlayerCompany = company;
                    OnPlayerCompanyChanged?.Invoke(this, PlayerCompany);
                }
            } else
            {
                if (CompanyDictionary.Dictionary.TryGetValue(tag, out var company))
                {
                    PlayerCompany = company;
                    OnPlayerCompanyChanged?.Invoke(this, PlayerCompany);
                }
            }
            PlayerCompany.isAI = false;
        }

        [MoonSharpHidden]
        public override void LoadRules()
        {
            scriptPath = "company/manager.lua";
            LoadScript();
            LoadCompanies();

            RaiseOnRulesLoaded();
        }

        [MoonSharpHidden]
        public override void LoadContent(string path)
        {
            LoadPlayerCompany(path);

            RaiseOnContentLoaded();
        }

        #region CONTENT LOADER
        private void LoadCompanies()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString("vanilla/company/companies.json");
            if (json == null) return;

            List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(json);
            
            CompanyDictionary.AddCompanies(companies);
            OnCompaniesLoaded?.Invoke(this, companies);
        }

        private void LoadPlayerCompany(string path)
        {
            string tag = Utils.ContentHandler.SafeGetString($"{path}/company/company.txt");
            if (tag == null) return;

            if (CompanyDictionary.Dictionary.TryGetValue(tag, out var company))
            {
                ChangeCompany(company.tag);
                OnPlayerCompanyLoaded?.Invoke(this, company);
            }
        }
        #endregion
    }
}
