#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
        private static void BeginWindow()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(-1);
            GUILayout.BeginVertical();
        }

        private static void EndWindow()
        {
            GUILayout.EndVertical();
            GUILayout.Space(-1);
            GUILayout.EndHorizontal();
        }
    }
}
#endif