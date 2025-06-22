#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

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
        private static readonly Color s_backgroundColorPro = new(0.22f, 0.22f, 0.22f);
        private static readonly Color s_backgroundColorLight = new(0.76f, 0.76f, 0.76f);
        private static readonly Color s_highlightColorPro = new(0.24f, 0.37f, 0.58f);
        private static readonly Color s_highlightColorLight = new(0.22f, 0.44f, 0.9f);

        public Color BorderColor;
        public Color HighlightColor;
        public Color BackgroundColor;

        public EditorWindowDrawer()
        {
            var size = s_minSize;
            var position = GetMousePosition(true, size);

            _desiredPosition = new Rect(position.x, position.y, size.x, size.y);

            base.minSize = s_minSize;

            InitializeColors();
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

            InitializeColors();
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

        private void InitializeColors()
        {
            BorderColor = EditorGUIUtility.isProSkin ? s_borderColorPro : s_borderColorLight;
            HighlightColor = EditorGUIUtility.isProSkin ? s_highlightColorPro : s_highlightColorLight;
            BackgroundColor = EditorGUIUtility.isProSkin ? s_backgroundColorPro : s_backgroundColorLight;
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

        public float SplitterPosition = 300f;
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
                    GUILayout.BeginHorizontal();
                    DrawPane(_paneStyle);
                    DrawSplitter(_paneStyle);
                    GUILayout.Space(-SplitterSize);
                    DrawBody(_paneStyle);
                    GUILayout.EndHorizontal();
                    break;
                case EditorPaneStyle.Right:
                    GUILayout.BeginHorizontal();
                    DrawBody(_paneStyle);
                    DrawSplitter(_paneStyle);
                    DrawPane(_paneStyle);
                    GUILayout.EndHorizontal();
                    break;
                case EditorPaneStyle.Top:
                    DrawPane(_paneStyle, false);
                    DrawSplitter(_paneStyle);
                    DrawBody(_paneStyle);
                    break;
                case EditorPaneStyle.Bottom:
                    DrawBody(_paneStyle, false);
                    DrawSplitter(_paneStyle);
                    DrawPane(_paneStyle);
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
                    GUILayout.ExpandWidth(false),
                    GUILayout.Height(SplitterSize));
            else hitboxSplitter = GUILayoutUtility.GetRect(SplitterSize, float.MaxValue,
                    GUILayout.ExpandHeight(false),
                    GUILayout.Width(SplitterSize));

            var visibleSplitter = isVerticalOrientation
                ? new Rect(hitboxSplitter.x, hitboxSplitter.y, hitboxSplitter.width, 1)
                : new Rect(hitboxSplitter.x, hitboxSplitter.y, 1, hitboxSplitter.height);

            EditorGUI.DrawRect(visibleSplitter, BorderColor);

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
                    float headerOffset = _headerAction != null ? 21 : 0;
                    float mouseY = GetLocalMousePosition().y - headerOffset;
                    float minPaneHeight = MinSplitterSize;
                    float maxPaneHeight = Position.height - MinSplitterSize - SplitterSize;
                    SplitterPosition = Mathf.Clamp(mouseY, minPaneHeight, maxPaneHeight);
                }
                else
                {
                    float mouseX = GetLocalMousePosition().x;
                    float minPaneWidth = MinSplitterSize;
                    float maxPaneWidth = Position.width - MinSplitterSize - SplitterSize;
                    float splitterPosition = Mathf.Clamp(mouseX, minPaneWidth, maxPaneWidth);
                    SplitterPosition = paneFirst ? splitterPosition : Position.width - splitterPosition;
                }

                Repaint();
            }
        }

        private void DrawPane(EditorPaneStyle paneStyle, bool expandHeight = true)
        {
            bool isVerticalOrientation = paneStyle == EditorPaneStyle.Top || paneStyle == EditorPaneStyle.Bottom;
            if (isVerticalOrientation)
                BeginPaneVerticalLayout(_paneSkin, Position.width, expandHeight, SplitterPosition, ref PaneScrollPosition);
            else BeginPane(_paneSkin, Position.height, SplitterPosition, ref PaneScrollPosition);

            _paneAction?.Invoke();
            EndPane();
        }

        private static void BeginPane(EditorWindowStyle skin, float height, float splitterPosition, ref Vector2 scrollPosition)
        {
            GUILayoutOption[] guiLayoutOptions = { GUILayout.Height(height) };

            GUILayout.BeginVertical(GetStyle(skin), GUILayout.Width(splitterPosition));
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, guiLayoutOptions);
        }

        private static void BeginPaneVerticalLayout(EditorWindowStyle skin, float width, bool expandHeight, float splitterPosition, ref Vector2 scrollPosition)
        {
            var guiLayoutOptions = new List<GUILayoutOption>() { GUILayout.Width(width) };

            if (expandHeight)
                guiLayoutOptions.Add(GUILayout.ExpandHeight(true));
            else guiLayoutOptions.Add(GUILayout.Height(splitterPosition));

            GUILayout.BeginVertical(GetStyle(skin), guiLayoutOptions.ToArray());
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        }

        private static void EndPane()
        {
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawBody(EditorPaneStyle? paneStyle = null, bool expandHeight = true)
        {
            bool isVerticalOrientation = paneStyle == EditorPaneStyle.Top || paneStyle == EditorPaneStyle.Bottom;
            if (isVerticalOrientation)
                BeginBodyVerticalLayout(_bodySkin, Position.width, expandHeight, SplitterPosition, ref BodyScrollPosition);
            else BeginBody(_bodySkin, Position.height, paneStyle == null, ref BodyScrollPosition);

            _bodyAction?.Invoke();

            if(paneStyle != null)
            {
                GUILayout.FlexibleSpace();

                BeginFooter(_footerSkin, isVerticalOrientation || paneStyle == EditorPaneStyle.Right);
                _footerAction?.Invoke();
                EndFooter(_footerSkin, isVerticalOrientation);

                if (!isVerticalOrientation)
                    GUILayout.Space(23);
            }

            EndBody();
        }

        private static void BeginBody(EditorWindowStyle skin, float height, bool expandHeight, ref Vector2 scrollPosition)
        {
            var guiLayoutOptions = new List<GUILayoutOption>();

            if (expandHeight)
                guiLayoutOptions.Add(GUILayout.ExpandHeight(true));
            else guiLayoutOptions.Add(GUILayout.Height(height));

            GUILayout.BeginVertical(GetStyle(skin), GUILayout.ExpandWidth(true));
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, guiLayoutOptions.ToArray());

            if (skin == EditorWindowStyle.Window)
                GUILayout.Space(-17);
        }

        private static void BeginBodyVerticalLayout(EditorWindowStyle skin, float width, bool expandHeight, float splitterPosition, ref Vector2 scrollPosition)
        {
            var guiLayoutOptions = new List<GUILayoutOption>() { GUILayout.Width(width) };

            if (expandHeight)
                guiLayoutOptions.Add(GUILayout.ExpandHeight(true));
            else guiLayoutOptions.Add(GUILayout.Height(splitterPosition));

            GUILayout.BeginVertical(GetStyle(skin), guiLayoutOptions.ToArray());
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

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
            if (_paneAction != null)
                return;

            BeginFooter(_footerSkin, false);
            _footerAction?.Invoke();
            EndFooter(_footerSkin, false);
        }

        private static void BeginFooter(EditorWindowStyle skin, bool isVerticalOrientation)
        {
            if (skin == EditorWindowStyle.HelpBox)
                GUILayout.Space(-2);

            if (skin == EditorWindowStyle.Window)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(3);
            }

            GUILayout.BeginVertical(GetStyle(skin));

            if (skin == EditorWindowStyle.Window)
                GUILayout.Space(-17);
        }

        private static void EndFooter(EditorWindowStyle skin, bool isVerticalOrientation)
        {
            if (skin == EditorWindowStyle.Window)
            {
                GUILayout.EndVertical();
                GUILayout.Space(isVerticalOrientation ? 2 : 4);
            }

            GUILayout.EndHorizontal();

            if (skin == EditorWindowStyle.Window && isVerticalOrientation)
                GUILayout.Space(2);
        }

        private void BeginDrawBorder(bool drawBorder)
        {
            if (!drawBorder)
                return;

            EditorGUI.DrawRect(new Rect(0, 0, Position.width, Position.height), BorderColor);
            EditorGUI.DrawRect(new Rect(1, 1, Position.width - 2, Position.height - 2), BackgroundColor);
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