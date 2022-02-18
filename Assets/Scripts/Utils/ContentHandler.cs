using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using Load;
using Invesment;
using Time;
using Game;
using Investor;
using UI;
using Language;
using Map;
using Ownership;
using SaveFile;
using Loan;
using District;

namespace Utils
{
    [MoonSharpUserData]
    public static class ContentHandler
    {
        public static Script SafeGetScript(string path)
        {
            string json = SafeGetString(path);
            if (json == null) return null;

            UserData.RegisterType<TMPro.TextMeshProUGUI>();
            UserData.RegisterType<TMPro.TMP_InputField>();
            UserData.RegisterType<Transform>();
            UserData.RegisterType<GameObject>();
            UserData.RegisterType<Image>();
            UserData.RegisterType<RawImage>();
            UserData.RegisterType<Color>();
            UserData.RegisterType<Texture2D>();
            
            UserData.RegisterType<EventArgs>();
            
            UserData.RegisterAssembly();
            Script script = new Script();
            script.Globals["LoadManager"] = LoadManager.Instance;
            script.Globals["GameManager"] = GameManager.Instance;
            script.Globals["TimeManager"] = TimeManager.Instance;
            script.Globals["InvesmentManager"] = InvesmentManager.Instance;
            script.Globals["InvestorManager"] = InvestorManager.Instance;
            script.Globals["UIManager"] = UIManager.Instance;
            script.Globals["LanguageManager"] = LanguageManager.Instance;
            script.Globals["MapManager"] = MapManager.Instance;
            script.Globals["OwnershipManager"] = OwnershipManager.Instance;
            script.Globals["SaveFileManager"] = SaveFileManager.Instance;
            script.Globals["LoanManager"] = LoanManager.Instance;
            script.Globals["DistrictManager"] = DistrictManager.Instance;

            script.Globals["InvesmentDictionary"] = new InvesmentDictionary();
            script.Globals["OwnershipList"] = new OwnershipList();
            script.Globals["InvestorList"] = new InvestorList();
            script.Globals["DistrictDictionary"] = new DistrictDictionary();

            script.Globals["ConsoleRunCommand"] = (Action<string>)Console.Console.Run;
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

