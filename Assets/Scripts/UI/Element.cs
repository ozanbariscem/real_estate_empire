using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{ 
    public class Element : MonoBehaviour
    {
        private Script script;

        [MoonSharpHidden]
        public void SetScript(Script script)
        {
            this.script = script;

            this.script.Call(this.script.Globals["OnScriptSet"], transform);

            SetOnClicks();
        }

        private void SetOnClicks()
        {
            Table table = script.Globals["onClicks"] as Table;

            for (int i = 1; i <= table.Length; i++)
            {
                Table pair = table[i] as Table;
                string button = pair[1] as string;
                string function = pair[2] as string;

                transform.Find(button).GetComponent<Button>().onClick.AddListener(
                    () => script.Call(script.Globals[function]));
            }
            this.script.Call(this.script.Globals["OnClickEventsSet"]);
        }
    }
}