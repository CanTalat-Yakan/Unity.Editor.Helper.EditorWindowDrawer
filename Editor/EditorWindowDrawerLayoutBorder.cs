#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
        private void BeginDrawBorder(bool drawBorder)
        {
            if (!drawBorder)
                return;

            EditorGUI.DrawRect(new Rect(0, 0, Position.width, Position.height), DarkColor);
            EditorGUI.DrawRect(new Rect(1, 1, Position.width - 2, Position.height - 2), NormalColor);
            GUILayout.BeginArea(new Rect(1, 1, Position.width - 2, Position.height - 2));
        }

        private void EndDrawBorder(bool drawBorder)
        {
            if (!drawBorder)
                return;

            GUILayout.EndArea();
        }
    }
}
#endif