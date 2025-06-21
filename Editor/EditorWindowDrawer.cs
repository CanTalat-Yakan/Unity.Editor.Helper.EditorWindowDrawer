#if UNITY_EDITOR
using System;
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

    public class EditorWindowDrawer : EditorWindow
    {
        public Rect Position => base.position;
        public Vector2 BodyScrollPosition;
        public Vector2 PaneScrollPosition;

        private string _desiredTitle;
        private Rect _desiredPosition;

        private static readonly Vector2 s_minSize = new(256, 256);
        private static readonly Color s_borderColorPro = new(0.15f, 0.15f, 0.15f);
        private static readonly Color s_borderColorLight = new(0.65f, 0.65f, 0.65f);

        public EditorWindowDrawer()
        {
            var size = s_minSize;
            var position = GetMousePosition(true, size);

            _desiredPosition = new Rect(position.x, position.y, size.x, size.y);

            base.minSize = s_minSize;
        }

        public EditorWindowDrawer(string title = null, Vector2? minSize = null, Vector2? size = null, Vector2? position = null, bool centerPosition = true)
        {
            size ??= s_minSize;
            minSize ??= s_minSize;
            position ??= GetMousePosition(centerPosition, size);

            _desiredTitle = string.IsNullOrEmpty(title) ? string.Empty : title;
            _desiredPosition = new Rect(position.Value.x, position.Value.y, size.Value.x, size.Value.y);

            base.titleContent = new GUIContent(_desiredTitle);
            base.minSize = minSize.Value;
        }

        public EditorWindowDrawer ShowWindow()
        {
            base.Show();
            base.Focus();
            base.position = _desiredPosition;
            _initialization?.Invoke();
            return this;
        }

        public EditorWindowDrawer ShowUtility()
        {
            base.ShowUtility();
            base.Focus();
            base.position = _desiredPosition;
            base.titleContent = new GUIContent(_desiredTitle);
            _initialization?.Invoke();
            return this;
        }

        public EditorWindowDrawer ShowPopup()
        {
            base.ShowPopup();
            base.Focus();
            _isUnfocusable = true;
            _initialization?.Invoke();
            return this;
        }

        public EditorWindowDrawer ShowAsDropDown(Rect rect, Vector2? size)
        {
            size ??= s_minSize;
            base.ShowAsDropDown(GUIUtility.GUIToScreenRect(rect), size.Value);
            base.Focus();
            _isUnfocusable = true;
            _initialization?.Invoke();
            return this;
        }

        private bool _drawBorder;
        public EditorWindowDrawer SetDrawBorder()
        {
            _drawBorder = true;
            return this;
        }


        private EditorApplication.CallbackFunction _editorApplicationCallback;
        public EditorWindowDrawer AddUpdate(Action updateAction)
        {
            RemoveUpdate(); // Ensure no duplicate
            _editorApplicationCallback = new(updateAction);
            EditorApplication.update += _editorApplicationCallback;
            return this;
        }

        public void RemoveUpdate()
        {
            if (_editorApplicationCallback != null)
            {
                EditorApplication.update -= _editorApplicationCallback;
                _editorApplicationCallback = null;
            }
        }

        private Action _initialization;
        public EditorWindowDrawer SetInitialization(Action initialization)
        {
            _initialization = initialization;
            return this;
        }

        private Action _preProcessAction;
        public EditorWindowDrawer SetPreProcess(Action preProcess)
        {
            _preProcessAction = preProcess;
            return this;
        }

        private Action _postProcessAction;
        public EditorWindowDrawer SetPostProcess(Action postProcess)
        {
            _postProcessAction = postProcess;
            return this;
        }

        private Action _headerAction;
        private EditorWindowStyle _headerSkin;
        public EditorWindowDrawer SetHeader(Action header, EditorWindowStyle skin = EditorWindowStyle.None)
        {
            _headerAction = header;
            _headerSkin = skin;
            return this;
        }

        private Action _paneAction;
        private EditorPaneStyle _paneStyle;
        private EditorWindowStyle _paneSkin;
        public EditorWindowDrawer SetPane(Action pane, EditorPaneStyle style = EditorPaneStyle.Left, EditorWindowStyle skin = EditorWindowStyle.None)
        {
            _paneAction = pane;
            _paneStyle = style;
            _paneSkin = skin;
            return this;
        }

        private Action _bodyAction;
        private EditorWindowStyle _bodySkin;
        public EditorWindowDrawer SetBody(Action body, EditorWindowStyle skin = EditorWindowStyle.None)
        {
            _bodyAction = body;
            _bodySkin = skin;
            return this;
        }

        private Action _footerAction;
        private EditorWindowStyle _footerSkin;
        public EditorWindowDrawer SetFooter(Action footer, EditorWindowStyle skin = EditorWindowStyle.None)
        {
            _footerAction = footer;
            _footerSkin = skin;
            return this;
        }

        public EditorWindowDrawer GetCloseEvent(out Action closeEvent)
        {
            closeEvent = base.Close;
            return this;
        }

        public EditorWindowDrawer GetRepaintEvent(out Action repaintEvent)
        {
            repaintEvent = base.Repaint;
            return this;
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
            RemoveUpdate();
            _preProcessAction = null;
            _postProcessAction = null;
            _headerAction = null;
            _paneAction = null;
            _bodyAction = null;
            _footerAction = null;
            _initialization = null;
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

        private void DrawHeader()
        {
            BeginHeader(_headerSkin);
            _headerAction?.Invoke();
            EndHeader(_headerSkin);
        }

        private static void BeginHeader(EditorWindowStyle skin)
        {
            if (skin == EditorWindowStyle.Toolbar)
                GUILayout.BeginHorizontal(GetStyle(skin));
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

        private void DrawBodySplitView()
        {
            if (_paneAction == null)
            {
                DrawBody();
                return;
            }

            switch (_paneStyle)
            {
                case EditorPaneStyle.Left:
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    DrawPane(_paneStyle);
                    DrawSplitter(_paneStyle);
                    DrawBody(_paneStyle);
                    GUILayout.EndHorizontal();
                    break;
                case EditorPaneStyle.Right:
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    DrawBody(_paneStyle);
                    DrawSplitter(_paneStyle);
                    DrawPane(_paneStyle);
                    GUILayout.EndHorizontal();
                    break;
                case EditorPaneStyle.Top:
                    GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                    DrawPane(_paneStyle);
                    DrawSplitter(_paneStyle);
                    DrawBody(_paneStyle);
                    GUILayout.EndVertical();
                    break;
                case EditorPaneStyle.Bottom:
                    GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                    DrawBody(_paneStyle);
                    DrawSplitter(_paneStyle);
                    DrawPane(_paneStyle);
                    GUILayout.EndVertical();
                    break;
            }
        }

        private void DrawBody(EditorPaneStyle? style = null)
        {
            var width = style == EditorPaneStyle.Right || style == EditorPaneStyle.Top ? Position.width : (float?)null;
            switch (style)
            {
                case EditorPaneStyle.Left:
                case EditorPaneStyle.Right:
                case null:
                    BeginBody(_bodySkin, width, ref BodyScrollPosition);
                    _bodyAction?.Invoke();
                    EndBody();
                    break;
                case EditorPaneStyle.Top:
                case EditorPaneStyle.Bottom:
                    BeginBodyVertical(_bodySkin, width, ref SlitterPosition, ref BodyScrollPosition);
                    _bodyAction?.Invoke();
                    EndBody();
                    break;
            }
        }

        public float SlitterPosition = 300f;
        private void DrawPane(EditorPaneStyle style)
        {
            switch (style)
            {
                case EditorPaneStyle.Left:
                case EditorPaneStyle.Right:
                    BeginPane(_paneSkin, ref SlitterPosition, ref PaneScrollPosition);
                    _paneAction?.Invoke();
                    EndPane();
                    break;
                case EditorPaneStyle.Top:
                case EditorPaneStyle.Bottom:
                    BeginPaneVertical(_paneSkin, Position.width, ref SlitterPosition, ref PaneScrollPosition);
                    _paneAction?.Invoke();
                    EndPane();
                    break;
            }
        }

        private bool _isDraggingSplitter;
        private const float MinSplitterSize = 100f;
        private const float SplitterSize = 5f;
        private void DrawSplitter(EditorPaneStyle style)
        {
            bool isVerticalOrientation = style == EditorPaneStyle.Top || style == EditorPaneStyle.Bottom;
            bool paneFirst = style == EditorPaneStyle.Left || style == EditorPaneStyle.Top;

            Rect hitboxSplitter;
            if (isVerticalOrientation)
                hitboxSplitter = GUILayoutUtility.GetRect(float.MaxValue, SplitterSize,
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(SplitterSize));
            else hitboxSplitter = GUILayoutUtility.GetRect(SplitterSize, float.MaxValue,
                    GUILayout.ExpandHeight(true),
                    GUILayout.Width(SplitterSize));

            var visibleSplitter = isVerticalOrientation
                ? new Rect(hitboxSplitter.x, hitboxSplitter.y, hitboxSplitter.width, 1)
                : new Rect(hitboxSplitter.x, hitboxSplitter.y, 1, hitboxSplitter.height);

            var borderColor = EditorGUIUtility.isProSkin ? s_borderColorPro : s_borderColorLight;
            EditorGUI.DrawRect(visibleSplitter, borderColor);

            EditorGUIUtility.AddCursorRect(hitboxSplitter, isVerticalOrientation
                ? MouseCursor.ResizeVertical
                : MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown)
                if (hitboxSplitter.Contains(GetLocalMousePosition()))
                    _isDraggingSplitter = true;

            if (Event.current.type == EventType.MouseUp)
                _isDraggingSplitter = false;

            if (_isDraggingSplitter)
            {
                if (isVerticalOrientation)
                {
                    float mouseY = GetLocalMousePosition().y;
                    float minPaneHeight = MinSplitterSize;
                    float maxBodyHeight = Position.height - MinSplitterSize - SplitterSize;
                    float maxPaneHeight = Position.height - MinSplitterSize - SplitterSize;
                    float splitterPosition = Mathf.Clamp(mouseY, minPaneHeight, maxPaneHeight);
                    SlitterPosition = paneFirst ? splitterPosition : Position.height - splitterPosition;
                }
                else
                {
                    float mouseX = GetLocalMousePosition().x;
                    float minPaneWidth = MinSplitterSize;
                    float maxPaneWidth = Position.width - MinSplitterSize - SplitterSize;
                    float splitterPosition = Mathf.Clamp(mouseX, minPaneWidth, maxPaneWidth);
                    SlitterPosition = paneFirst ? splitterPosition : Position.width - splitterPosition;
                }

                Repaint();
            }
        }

        private static void BeginPane(EditorWindowStyle skin, ref float splitterPosition, ref Vector2 scrollPosition)
        {
            GUILayout.BeginVertical(GetStyle(skin),
                GUILayout.Width(splitterPosition));
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));
        }

        private static void BeginPaneVertical(EditorWindowStyle skin, float width, ref float splitterPosition, ref Vector2 scrollPosition)
        {
            GUILayout.BeginVertical(GetStyle(skin),
                GUILayout.Height(splitterPosition),
                GUILayout.Width(width));
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));
        }

        private static void EndPane()
        {
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private static void BeginBodyVertical(EditorWindowStyle skin, float? width, ref float splitterPosition, ref Vector2 scrollPosition)
        {
            var guiLayoutOptions = width.HasValue
                ? new[] { GUILayout.Height(splitterPosition), GUILayout.Width(width.Value) }
                : new[] { GUILayout.Height(splitterPosition) };
            GUILayout.BeginVertical(GetStyle(skin), guiLayoutOptions);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            if (skin == EditorWindowStyle.Window)
                GUILayout.Space(-17);
        }

        private static void BeginBody(EditorWindowStyle skin, float? width, ref Vector2 scrollPosition)
        {
            var guiLayoutOptions = width.HasValue
                ? new[] { GUILayout.ExpandWidth(true), GUILayout.Width(width.Value) }
                : new[] { GUILayout.ExpandWidth(true) };
            GUILayout.BeginVertical(GetStyle(skin), guiLayoutOptions);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            if (skin == EditorWindowStyle.Window)
                GUILayout.Space(-17);
        }

        private static void EndBody()
        {
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawFooter()
        {
            BeginFooter(_footerSkin);
            _footerAction?.Invoke();
            EndFooter();
        }

        private static void BeginFooter(EditorWindowStyle skin)
        {
            if (skin == EditorWindowStyle.HelpBox)
                GUILayout.Space(-2);

            GUILayout.BeginVertical(GetStyle(skin));

            if (skin == EditorWindowStyle.Window)
                GUILayout.Space(-17);
        }

        private static void EndFooter() =>
            GUILayout.EndVertical();

        private void BeginDrawBorder(bool drawBorder)
        {
            if (!drawBorder)
                return;

            var borderColor = EditorGUIUtility.isProSkin ? s_borderColorPro : s_borderColorLight;
            EditorGUI.DrawRect(new Rect(0, 0, Position.width, Position.height), borderColor);
            EditorGUI.DrawRect(new Rect(1, 1, Position.width - 2, Position.height - 2), borderColor);
            GUILayout.BeginArea(new Rect(1, 1, Position.width - 2, Position.height - 2));
        }

        private void EndDrawBorder(bool drawBorder)
        {
            if (!drawBorder)
                return;

            GUILayout.EndArea();
        }

        private Vector2 GetMousePosition(bool centerPosition = true, Vector2? positionOffset = null)
        {
            var offset = Vector2.zero;
            if (positionOffset.HasValue && centerPosition)
            {
                offset = positionOffset.Value;
                offset /= 2;
            }
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