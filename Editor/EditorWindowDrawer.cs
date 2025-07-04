#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public enum EditorWindowStyle
    {
        None,
        Margin,
        BigMargin,
        Box,
        Window,
        HelpBox,
        Toolbar,
        Dark,
        Light
    }

    public enum EditorPaneStyle
    {
        Left,
        Top,
        Right,
        Bottom,
    }

    public partial class EditorWindowDrawer : EditorWindow
    {
        public Rect Position => base.position;
        public Vector2 BodyScrollPosition;
        public Vector2 PaneScrollPosition;
        public bool ContextMenuHandled = false;

        public static Color NormalColor;
        public static Color HighlightColor;
        public static Color LightColor;
        public static Color DarkColor;
        public static Color BlackColor;
        public static Color BorderColor;

        private string _desiredTitle;
        private Rect _desiredPosition;

        private static readonly Vector2 s_minSize = new(256, 256);

        public EditorWindowDrawer()
        {
            var size = s_minSize;
            var position = GetMousePosition(size);

            _desiredPosition = new Rect(position.x, position.y, size.x, size.y);

            base.minSize = s_minSize;
        }

        public EditorWindowDrawer(
            string title = null,
            Vector2? minSize = null,
            Vector2? size = null,
            Vector2? position = null,
            bool centerX = true,
            bool centerY = false)
        {
            size ??= s_minSize;
            minSize ??= s_minSize;
            position ??= GetMousePosition(size.Value, centerX, centerY);

            _desiredTitle = string.IsNullOrEmpty(title) ? string.Empty : title;
            _desiredPosition = new Rect(position.Value.x, position.Value.y, size.Value.x, size.Value.y);

            base.titleContent = new GUIContent(_desiredTitle);
            base.minSize = minSize.Value;
        }

        private void OnGUI()
        {
            _preProcessAction?.Invoke();

            BeginDrawBorder(_drawBorder);
            BeginWindow();
            DrawHeader();
            DrawBodySplitView();
            DrawFooter();
            EndWindow();
            EndDrawBorder(_drawBorder);

            _postProcessAction?.Invoke();
        }

        private static readonly Color s_defaultColorPro = new(0.22f, 0.22f, 0.22f);
        private static readonly Color s_defaultColorLight = new(0.78f, 0.78f, 0.78f);
        private static readonly Color s_highlightColorPro = new(0.24f, 0.37f, 0.59f);
        private static readonly Color s_highlightColorLight = new(0.245f, 0.49f, 0.905f);
        private static readonly Color s_lightColorPro = new(0.25f, 0.25f, 0.25f);
        private static readonly Color s_lightColorLight = new(0.81f, 0.81f, 0.81f);
        private static readonly Color s_darkColorPro = new(0.2f, 0.2f, 0.2f);
        private static readonly Color s_darkColorLight = new(0.745f, 0.745f, 0.745f);
        private static readonly Color s_blackColorPro = new(0.08f, 0.08f, 0.08f);
        private static readonly Color s_blackColorLight = new(0.745f, 0.745f, 0.745f);

        private static Color? s_defaultColor;
        private static Color? s_highlightColor;
        private static Color? s_lightColor;
        private static Color? s_darkColor;
        private static Color? s_blackColor;

        protected virtual void OnEnable()
        {
            NormalColor = s_defaultColor ??= EditorGUIUtility.isProSkin ? s_defaultColorPro : s_defaultColorLight;
            HighlightColor = s_highlightColor ??= EditorGUIUtility.isProSkin ? s_highlightColorPro : s_highlightColorLight;
            LightColor = s_lightColor ??= EditorGUIUtility.isProSkin ? s_lightColorPro : s_lightColorLight;
            DarkColor = s_darkColor ??= EditorGUIUtility.isProSkin ? s_darkColorPro : s_darkColorLight;
            BlackColor = s_blackColor ??= EditorGUIUtility.isProSkin ? s_blackColorPro : s_blackColorLight;
            BorderColor = BlackColor * 0.6f;

            AddUpdate(() =>
            {
                if (_headerAction == null && _bodyAction == null && _paneAction == null && _footerAction == null)
                {
                    RemoveUpdate();
                    Close();
                }
            });
        }

        private bool _isUnfocusable = false;
        public void OnLostFocus()
        {
            if (_isUnfocusable)
                Close();

            RemoveUpdate();
        }

        public void OnDestroy()
        {
            _preProcessAction = null;
            _postProcessAction = null;
            _headerAction = null;
            _paneAction = null;
            _bodyAction = null;
            _footerAction = null;
            _initialization = null;

            RemoveUpdate();
        }
    }
}
#endif