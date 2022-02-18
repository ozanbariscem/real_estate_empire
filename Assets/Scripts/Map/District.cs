using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;

namespace Map 
{
    [MoonSharpUserData]
    public class District : MonoBehaviour
    {
        public enum State { CLOSING, CLOSED, OPENING, OPEN }

        public event EventHandler<District> OnClicked;
        public event EventHandler<District> OnDoubleClicked;

        public string district_tag;

        private Texture2D meshTexture;

        private Transform mesh;
        private Transform model;
        private Transform border;
        private Transform canvas;

        #region UI ELEMENTS
        private TextMeshProUGUI nameText;
        #endregion

        private State state;

        private float transitionSpeed = 2f;

        private float doubleClickRegisterTime;
        private float lastClickTime;

        private void Start()
        {
            doubleClickRegisterTime = 0.2f;
            transitionSpeed = 4f;

            district_tag = name;

            Subscribe();
            GetComponents();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Update()
        {
            if (IsClicked()) OnClicked?.Invoke(this, this);

            switch (state)
            {
                case State.CLOSING:
                    Transition(-1);
                    if (model.localScale.y <= 0.000001f)
                    {
                        model.localScale = new Vector3(model.localScale.x, 0.001f, model.localScale.z);
                        state = State.CLOSED;
                    }
                    break;
                case State.CLOSED:
                    break;
                case State.OPENING:
                    Transition(1);
                    if (model.localScale.y >= 1)
                    {
                        model.localScale = new Vector3(model.localScale.x, 1, model.localScale.z);
                        state = State.OPEN;
                    }
                    break;
                case State.OPEN:
                    break;
            }
        }

        private bool IsClicked()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000f);
                
                foreach (var hit in hits)
                {
                    if (hit.transform == mesh)
                    {
                        int x = (int)(hit.textureCoord.x * meshTexture.width);
                        int y = (int)(hit.textureCoord.y * meshTexture.height);
                        Color pixel = meshTexture.GetPixel(x, y);
                        
                        if (pixel.a > 0) return true;
                    }
                }
            }
            return false;
        }

        private void GetComponents()
        {
            mesh = transform.Find("Mesh");
            model = transform.Find("Model");
            border = transform.Find("Border");
            canvas = transform.Find("Canvas");

            nameText = canvas.Find("Text").GetComponent<TextMeshProUGUI>();
            meshTexture = mesh.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
        }

        public void UpdateName(string name)
        {
            nameText.text = name;
        }

        private void UpdateCanvas()
        {
            
        }

        private void Transition(int direction)
        {
            if (direction == 0) return;
            direction = Mathf.Clamp(direction, -1, 1);

            model.localScale += new Vector3(0, direction * transitionSpeed * UnityEngine.Time.deltaTime, 0);
        }

        private void HandleOnClicked(object sender, District district)
        {
            if (UnityEngine.Time.time - lastClickTime <= doubleClickRegisterTime)
            { 
                OnDoubleClicked?.Invoke(this, district);
            }

            lastClickTime = UnityEngine.Time.time;

            Show();
        }

        private void HandleOnDoubleClicked(object sender, District district)
        {
            
        }

        public void Show()
        {
            switch (state)
            {
                case State.CLOSED:
                case State.CLOSING:
                    state = State.OPENING;
                    break;
            }
        }

        public void Hide()
        {
            state = State.CLOSING;
        }

        private void Subscribe()
        {
            OnClicked += HandleOnClicked;
            OnDoubleClicked += HandleOnDoubleClicked;
        }

        private void Unsubscribe()
        {
            OnClicked -= HandleOnClicked;
            OnDoubleClicked -= HandleOnDoubleClicked;
        }
    }
}


