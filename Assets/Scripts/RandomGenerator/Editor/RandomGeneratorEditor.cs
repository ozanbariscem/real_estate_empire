#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Map.Editor
{
    [CustomEditor(typeof(RandomGenerator))]
    public class Inspector : UnityEditor.Editor
    {
        private RandomGenerator editor => target as RandomGenerator;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Feed RNG"))
                editor.FeedRng();

            if (GUILayout.Button("Generate Random Buildings"))
                editor.GenerateRandomBuildings();
        }
    }
}
#endif