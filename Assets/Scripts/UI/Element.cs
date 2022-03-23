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

        public void Activate(object param)
        {
            if (!script.Globals.Get("OnActivate").IsNil())
                script.Call(script.Globals["OnActivate"], param);
            transform.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            if (!script.Call(script.Globals["OnDeactivate"]).IsNil())
                script.Call(script.Globals["OnDeactivate"], transform);
            transform.gameObject.SetActive(false);
        }

        [MoonSharpHidden]
        public void SetScript(Script script)
        {
            this.script = script;

            if (!this.script.Globals.Get("OnScriptSet").IsNil())
                this.script.Call(this.script.Globals["OnScriptSet"], transform);

            SetOnClicks();
            SetOnHovers();
            SetOnValueChanges();
        }

        private void SetOnClicks()
        {
            if (script.Globals.Get("onClicks").IsNil()) return;

            Table table = script.Globals["onClicks"] as Table;

            for (int i = 1; i <= table.Length; i++)
            {
                Table pair = table[i] as Table;
                string buttonPath = pair[1] as string;
                string function = pair[2] as string;

                Button button = transform.Find(buttonPath).GetComponent<Button>();

                if (button != null)
                {
                    button.onClick.AddListener(
                        () => script.Call(script.Globals[function]));
                } else
                {
                    EventTrigger eventTrigger = transform.Find(buttonPath).gameObject.AddComponent<EventTrigger>();

                    EventTrigger.Entry onClick = new EventTrigger.Entry();
                    onClick.eventID = EventTriggerType.PointerClick;
                    onClick.callback.AddListener((eventData) =>
                    {
                        script.Call(script.Globals[function]);
                    });

                    eventTrigger.triggers.Add(onClick);
                }
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
    
        private void SetOnValueChanges()
        {
            if (script.Globals.Get("onValueChanges").IsNil()) return;

            Table table = script.Globals["onValueChanges"] as Table;

            for (int i = 1; i <= table.Length; i++)
            {
                Table pair = table[i] as Table;
                string inputPath = pair[1] as string;
                string function = pair[2] as string;

                TMPro.TMP_InputField field = transform.Find(inputPath).GetComponent<TMPro.TMP_InputField>();

                if (field != null)
                {
                    field.onValueChanged.AddListener(
                        (x) => script.Call(script.Globals[function], x));
                }
            }

            if (!script.Globals.Get("OnValueChangeEventsSet").IsNil())
                script.Call(script.Globals["OnValueChangeEventsSet"]);
        }
    }
}