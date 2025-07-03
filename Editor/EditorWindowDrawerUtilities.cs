#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
        private Vector2 GetMousePosition(Vector2 offset, bool centerX = true, bool centerY = true)
        {
            offset.x = centerX ? offset.x / 2 : 0;
            offset.y = centerY ? offset.y / 2 : 0;

            return MouseInputFetcher.CurrentMousePosition - offset;
        }

        public Vector2 GetLocalMousePosition() =>
            MouseInputFetcher.CurrentMousePosition - base.position.position;

        private static GUIStyle GetStyle(EditorWindowStyle skin)
        {
            return skin switch
            {
                EditorWindowStyle.Box => GUI.skin.box,
                EditorWindowStyle.Window => GUI.skin.window,
                EditorWindowStyle.HelpBox => EditorStyles.helpBox,
                EditorWindowStyle.Toolbar => EditorStyles.toolbar,
                EditorWindowStyle.Margin => EditorStyles.inspectorFullWidthMargins,
                EditorWindowStyle.BigMargin => EditorStyles.inspectorDefaultMargins,
                EditorWindowStyle.None or _ => GUIStyle.none,
            };
        }
    }
}
#endif