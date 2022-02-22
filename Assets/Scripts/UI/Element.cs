using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    [MoonSharpUserData]
    public class Element : MonoBehaviour
    {
        private Script script;

        [MoonSharpHidden]
        public void SetScript(Script script)
        {
            this.script = script;

            this.script.Call(this.script.Globals["OnScriptSet"], transform);

            SetOnClicks();
            SetOnHovers();
        }

        private void SetOnClicks()
        {
            if (script.Globals.Get("onClicks").IsNil()) return;

            Table table = script.Globals["onClicks"] as Table;

            for (int i = 1; i <= table.Length; i++)
            {
                Table pair = table[i] as Table;
                string button = pair[1] as string;
                string function = pair[2] as string;

                transform.Find(button).GetComponent<Button>().onClick.AddListener(
                    () => script.Call(script.Globals[function]));
            }

            if (!script.Globals.Get("OnClickEventsSet").IsNil())
                script.Call(script.Globals["OnClickEventsSet"]);
        }

        private void SetOnHovers()
        {
            if (script.Globals.Get("onHovers").IsNil()) return;

            Table table = script.Globals["onHovers"] as Table;

            for (int i = 1; i <= table.Length; i++)
            {
                Table pair = table[i] as Table;
                string name = pair[1] as string;
                string function = pair[2] as string;

                EventTrigger eventTrigger = transform.Find(name).gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry onEnter = new EventTrigger.Entry();
                onEnter.eventID = EventTriggerType.PointerEnter;
                onEnter.callback.AddListener((eventData) =>
                {
                    Table table = script.Call(script.Globals[function]).Table;

                    string header = table["header"] as string;
                    string description = table["description"] as string;

                    Hover.Instance.UpdateContent(header, description);
                });

                EventTrigger.Entry onExit = new EventTrigger.Entry();
                onExit.eventID = EventTriggerType.PointerExit;
                onExit.callback.AddListener((eventData) =>
                {
                    Hover.Instance.Hide();
                });

                eventTrigger.triggers.Add(onEnter);
                eventTrigger.triggers.Add(onExit);
            }

            if (!script.Globals.Get("OnHoverEventsSet").IsNil())
                script.Call(script.Globals["OnHoverEventsSet"]);
        }
    }
}