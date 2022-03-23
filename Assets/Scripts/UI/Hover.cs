using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace UI
{
    public class Hover : MonoBehaviour
    {
        public static Hover Instance { get; private set; }

        private float width;
        
        private Transform content;

        private TextMeshProUGUI header;
        private TextMeshProUGUI description;

        private RectTransform thisRect;
        private RectTransform contentRect;

        private Canvas canvas;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Start()
        {
            canvas = transform.parent.GetComponent<Canvas>();

            content = transform.Find("Content");
        
            header = content.Find("Header").GetComponent<TextMeshProUGUI>();
            description = content.Find("Description").GetComponent<TextMeshProUGUI>();

            thisRect = transform.GetComponent<RectTransform>();
            contentRect = content.GetComponent<RectTransform>();

            width = contentRect.rect.width;

            StartCoroutine(UpdateContentCoroutine("", ""));
            Hide();
        }

        private void Update()
        {
            PositionMenu();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateContent(string headerText, string descriptionText)
        {
            Show();
            thisRect.sizeDelta = Vector2.zero;
            content.gameObject.SetActive(false);

            StartCoroutine(UpdateContentCoroutine(headerText, descriptionText));
        }

        private IEnumerator UpdateContentCoroutine(string headerText, string descriptionText)
        {
            header.text = headerText;
            description.text = descriptionText;

            Vector2 size = description.GetPreferredValues();
            size.x = width;
            description.rectTransform.sizeDelta = size;
            description.ForceMeshUpdate();

            content.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            thisRect.sizeDelta = contentRect.sizeDelta + new Vector2(10, 10);
        }

        private void PositionMenu()
        {
            if (Input.mousePosition.x + (thisRect.sizeDelta.x * canvas.scaleFactor) > Screen.width)
            {
                thisRect.pivot = new Vector2(1, thisRect.pivot.y);
            } else
            {
                thisRect.pivot = new Vector2(0, thisRect.pivot.y);
            }
            if (thisRect.sizeDelta.y * canvas.scaleFactor > Input.mousePosition.y)
            {
                thisRect.pivot = new Vector2(thisRect.pivot.x, 0);
            } else
            {
                thisRect.pivot = new Vector2(thisRect.pivot.x, 1);
            }

            transform.position = Input.mousePosition + new Vector3(10,0,0);
        }
    }
}
