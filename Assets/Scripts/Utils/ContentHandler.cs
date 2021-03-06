using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using Load;
using Investment;
using Investment.Property;
using Time;
using Game;
using Company;
using UI;
using Language;
using Map;
using Ownership;
using SaveFile;
using Loan;
using District;
using Modifier;
using RandomEvent;
using Job;
using Employment;
using Person;
using Person.Employee;

namespace Utils
{
    [MoonSharpUserData]
    public static class ContentHandler
    {
        public static Script SafeGetScript(string path)
        {
            string json = SafeGetString(path);
            if (json == null) return null;

            UserData.RegisterType<UnityEngine.Events.UnityAction>();
            UserData.RegisterType<TMPro.TMP_InputField.OnChangeEvent>();
            
            UserData.RegisterType<TMPro.TextMeshProUGUI>();
            UserData.RegisterType<TMPro.TMP_InputField>();
            UserData.RegisterType<Transform>();
            UserData.RegisterType<GameObject>();
            UserData.RegisterType<Image>();
            UserData.RegisterType<RawImage>();
            UserData.RegisterType<RectTransform>();
            UserData.RegisterType<Vector2>();
            UserData.RegisterType<Vector3>();
            UserData.RegisterType<Color>();
            UserData.RegisterType<Texture2D>();
            UserData.RegisterType<Outline>();

            UserData.RegisterType<EventArgs>();
            
            UserData.RegisterAssembly();
            Script script = new Script();
            script.Globals["LoadManager"] = LoadManager.Instance;
            script.Globals["GameManager"] = GameManager.Instance;
            script.Globals["TimeManager"] = TimeManager.Instance;
            script.Globals["CompanyManager"] = CompanyManager.Instance;
            script.Globals["UIManager"] = UIManager.Instance;
            script.Globals["LanguageManager"] = LanguageManager.Instance;
            script.Globals["MapManager"] = MapManager.Instance;
            script.Globals["OwnershipManager"] = OwnershipManager.Instance;
            script.Globals["SaveFileManager"] = SaveFileManager.Instance;
            script.Globals["LoanManager"] = LoanManager.Instance;
            script.Globals["DistrictManager"] = DistrictManager.Instance;
            script.Globals["ModifierManager"] = ModifierManager.Instance;
            script.Globals["EventManager"] = EventManager.Instance;
            script.Globals["JobManager"] = JobManager.Instance;
            script.Globals["EmploymentManager"] = EmploymentManager.Instance;
            script.Globals["PersonManager"] = PersonManager.Instance;
            script.Globals["EmployeeManager"] = EmployeeManager.Instance;
            script.Globals[nameof(BuildingManager)] = BuildingManager.Instance;

            script.Globals[nameof(BuildingDictionary)] = typeof(BuildingDictionary);
            script.Globals[nameof(ApartmentDictionary)] = typeof(ApartmentDictionary);

            script.Globals["OwnershipDictionary"] = new OwnershipDictionary();
            script.Globals["CompanyDictionary"] = new CompanyDictionary();
            script.Globals["LoanDictionary"] = new LoanDictionary();
            script.Globals["DistrictDictionary"] = new DistrictDictionary();
            script.Globals["ModifierDictionary"] = new ModifierDictionary();
            script.Globals["EffectDictionary"] = new Effect.Effect();
            script.Globals["EventDictionary"] = new EventDictionary();
            script.Globals["JobDictionary"] = new JobDictionary();
            script.Globals["EmploymentDictionary"] = new EmploymentDictionary();
            script.Globals["EmployeeDictionary"] = new EmployeeDictionary();

            script.Globals["Company"] = typeof(Company.Company);
            script.Globals["MapDistrict"] = typeof(Map.District);
            script.Globals["Debug"] = REE.Debug.Debug.Instance;

            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;

            script.Globals["ToCashString"] = (Func<long, string>)StringConversions.ToCash;
            script.Globals["ToShortCashString"] = (Func<long, string>)StringConversions.ToShortCash;

            script.Globals["OpenYoutube"] = (Action)Web.Youtube;
            script.Globals["OpenTwitter"] = (Action)Web.Twitter;
            script.Globals["OpenDiscord"] = (Action)Web.Discord;
            script.DoString(json);

            return script;
        }

        public static string SafeGetString(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"Hey make sure {path} exists!");
                return null;
            }

            StreamReader sr = new StreamReader(path);
            string json = sr.ReadToEnd();
            sr.Close();

            return json;
        }

        public static Texture2D SafeGetTexture(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"Hey make sure {path} exists!");
                return null;
            }

            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            return texture;
        }

        public static void SafeSetString(string path, string content)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(content);
            sw.Close();
        }
    }
}

