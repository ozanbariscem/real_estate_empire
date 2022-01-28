using System;
using System.IO;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Console
{
    public class Console : MonoBehaviour
    {
        public event Action<string> OnHistoryIndexChanged;

        public string scriptPath = "vanilla/console/console.lua";
        public UI ui;

        private History history;

        Script lua;

        private void Start()
        {
            scriptPath = Path.Combine(Application.streamingAssetsPath, scriptPath);
            history = new History();
            LoadScript();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                history.DecreaseIndex();
                OnHistoryIndexChanged?.Invoke(history.GetLog());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                history.IncreaseIndex();
                OnHistoryIndexChanged?.Invoke(history.GetLog());
            }
        }

        // InputField on console prefab calls this
        public void HandleInputSubmit(string text)
        {
            // We split the given text to 2 pieces
            // First piece is the command itself
            // And any remaining piece is the arguments given
            string[] command = text.Split(new string[] { " " }, 2, StringSplitOptions.None);
            string functionName = command[0];

            // Find out if the Lua script has such function
            DynValue function = lua.Globals.Get(functionName);
            if (!function.IsNil())
            {
                // if you use a non-parameter command with parameters
                // lua interpreter just ignores it
                // so "clear 2 3 sasd d3" would still clear the log
                // and the reverse condition works as well
                // "add  " will return 0
                string[] args = new string[0];
                if (command.Length > 1) args = command[1].Split(' ');

                lua.Call(function, args);
            }

            if (text != "")
            {
                history.AddLog(text);
            }
        }

        private void LoadScript()
        {
            string json = Utils.StreamingAssetsHandler.SafeGetString(scriptPath);
            if (json == null) return;

            lua = new Script();

            // Don't forget to register the classes you are going to expose
            // If you want to only expose certain functions of a class
            // You could use proxies
            UserData.RegisterType<UI>();

            // You can expose any function or class as such
            // Function example
            lua.Globals["Log"] = (Action<string>)ui.AddToLog;
            lua.Globals["Random"] = (Func<int, int, int>)UnityEngine.Random.Range;
            // Class example
            lua.Globals["UI"] = ui;

            lua.DoString(json);
        }
    }
}
