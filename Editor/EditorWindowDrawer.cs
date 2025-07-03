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
        Toolbar
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

        public Color BorderColor;
        public Color HighlightColor;
        public Color BackgroundColor;

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

        private static readonly Color s_borderColorPro = new(0.15f, 0.15f, 0.15f);
        private static readonly Color s_borderColorLight = new(0.65f, 0.65f, 0.65f);
        private static readonly Color s_backgroundColorPro = new(0.22f, 0.22f, 0.22f);
        private static readonly Color s_backgroundColorLight = new(0.76f, 0.76f, 0.76f);
        private static readonly Color s_highlightColorPro = new(0.24f, 0.37f, 0.58f);
        private static readonly Color s_highlightColorLight = new(0.22f, 0.44f, 0.9f);

        private static Color? s_borderColor;
        private static Color? s_highlightColor;
        private static Color? s_backgroundColor;

        protected virtual void OnEnable()
        {
            BorderColor = s_borderColor ??= EditorGUIUtility.isProSkin ? s_borderColorPro : s_borderColorLight;
            HighlightColor = s_highlightColor ??= EditorGUIUtility.isProSkin ? s_highlightColorPro : s_highlightColorLight;
            BackgroundColor = s_backgroundColor ??= EditorGUIUtility.isProSkin ? s_backgroundColorPro : s_backgroundColorLight;

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