using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Console
{
    public class UI : MonoBehaviour
    {
        public static EventHandler OnConsoleFocused;
        public static EventHandler OnConsoleDefocused;

        [SerializeField] private TMP_InputField input;
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private TextMeshProUGUI log;
        [SerializeField] private GameObject content;

        private float logWidth, logHeight;

        public Console console;

        public KeyCode toggleKey = KeyCode.BackQuote;

        private void Start()
        {
            GetMinimumLogBounds();

            console.OnHistoryIndexChanged += HandleHistoryIndexChanged;
        }

        private void OnDestroy()
        {
            console.OnHistoryIndexChanged -= HandleHistoryIndexChanged;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                input.DeactivateInputField();
            }
            if (Input.GetKeyDown(toggleKey))
            {
                content.SetActive(!content.activeInHierarchy);

                if (content.activeInHierarchy)
                {
                    HandleInputSelect(""); 
                }
                else
                {
                    HandleInputDeselect("");
                }
            }
        }

        public void AddToLog(string text)
        {
            log.text += $"{text}\n";

            Vector2 size = log.GetPreferredValues();
            size.x = logWidth;
            size.y = Mathf.Max(logHeight, size.y);
            log.rectTransform.sizeDelta = size;
            log.ForceMeshUpdate();

            scrollbar.value = 0;
        }

        public void ClearLog()
        {
            log.text = "";
            scrollbar.size = 1;
            AddToLog("");
        }

        public void SetInputText(string text)
        {
            input.text = text;

            input.caretPosition = text.Length;
        }

        private void GetMinimumLogBounds()
        {
            logWidth = log.rectTransform.sizeDelta.x;
            logHeight = log.rectTransform.sizeDelta.y;
        }

        #region EVENTS
        public void HandleHistoryIndexChanged(string log)
        {
            SetInputText(log);
        }

        public void HandleInputSelect(string text)
        {
            input.ActivateInputField();

            OnConsoleFocused?.Invoke(this, EventArgs.Empty);
            // You need to deactivate the input of other GameObjects here.
            //CameraController.Singleton.ignoreAllInput = true; // This is just an example that was suitable for my case
        }

        public void HandleInputDeselect(string text)
        {
            OnConsoleDefocused?.Invoke(this, EventArgs.Empty);
            // You need to reactivate the input of other GameObjects here.
            //CameraController.Singleton.ignoreAllInput = false; // This is just an example that was suitable for my case
        }

        public void HandleInputSubmit(string text)
        {
            input.text = "";
            input.ActivateInputField();
        }
        #endregion
    }
}
