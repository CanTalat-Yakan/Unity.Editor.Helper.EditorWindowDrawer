#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
        private void DrawHeader()
        {
            if (_paneAction != null && _headerSkin != EditorWindowStyle.Toolbar)
                return;

            BeginHeader(_headerSkin, Position.width + 2);
            _headerAction?.Invoke();
            EndHeader(_headerSkin);
        }

        private static void BeginHeader(EditorWindowStyle skin, float width)
        {
            if (skin == EditorWindowStyle.Toolbar)
                GUILayout.BeginHorizontal(GetStyle(skin), GUILayout.Width(width));
            else GUILayout.BeginVertical(GetStyle(skin));

            if (skin == EditorWindowStyle.Window)
                GUILayout.Space(-17);
        }

        private static void EndHeader(EditorWindowStyle skin)
        {
            if (skin == EditorWindowStyle.Toolbar)
                GUILayout.EndHorizontal();
            else GUILayout.EndVertical();

            if (skin == EditorWindowStyle.HelpBox)
                GUILayout.Space(-2);
        }
    }
}
#endif