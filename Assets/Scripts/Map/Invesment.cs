using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace Map
{
    /// <summary>
    /// This is the world space representation of an invesment
    /// This is loaded with the map with assetbundles and MUST have it's id as it's name
    /// It only knows it's ID
    /// It only handles mouse clicks
    /// </summary>
    [MoonSharpUserData]
    public class Invesment : MonoBehaviour
    {
        public event EventHandler<Invesment> OnInvesmentClicked;

        private int _id;
        public int Id => _id;

        private string _tag;
        public string Tag => _tag;

        public void Set(string tag, int id)
        {
            _tag = tag;
            _id = id;
        }

        public void OnMouseDown()
        {
            OnInvesmentClicked?.Invoke(this, this);
        }
    }
}
