using System;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// This is the world space representation of an invesment
    /// This is loaded with the map with assetbundles and MUST have it's id as it's name
    /// It only knows it's ID
    /// It only handles mouse clicks
    /// </summary>
    public class Invesment : MonoBehaviour
    {
        public event Action<string, int> OnInvesmentClicked;

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
            OnInvesmentClicked?.Invoke(_tag, _id);
        }
    }
}
