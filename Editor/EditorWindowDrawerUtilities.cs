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
                EditorWindowStyle.Dark => DarkStyle,
                EditorWindowStyle.Light => LightStyle,
                EditorWindowStyle.Box => GUI.skin.box,
                EditorWindowStyle.Window => GUI.skin.window,
                EditorWindowStyle.HelpBox => EditorStyles.helpBox,
                EditorWindowStyle.Toolbar => EditorStyles.toolbar,
                EditorWindowStyle.Margin => EditorStyles.inspectorFullWidthMargins,
                EditorWindowStyle.BigMargin => EditorStyles.inspectorDefaultMargins,
                EditorWindowStyle.None or _ => GUIStyle.none,
            };
        }

        private static GUIStyle _darkStyle;
        public static GUIStyle DarkStyle => _darkStyle ??= new GUIStyle()
        {
            normal = new GUIStyleState() { background = GetBackgroundTexture(DarkColor) },
        };

        private static GUIStyle _lightStyle;
        public static GUIStyle LightStyle => _lightStyle ??= new GUIStyle()
        {
            normal = new GUIStyleState() { background = GetBackgroundTexture(LightColor) },
        };

        private static Texture2D _backgroundTexture;
        private static Texture2D GetBackgroundTexture(Color color)
        {
            if (_backgroundTexture == null)
            {
                _backgroundTexture = new Texture2D(1, 1);
                _backgroundTexture.hideFlags = HideFlags.HideAndDontSave;
            }
            _backgroundTexture.SetPixel(0, 0, color);
            _backgroundTexture.Apply();
            return _backgroundTexture;
        }
    }
}
#endif
