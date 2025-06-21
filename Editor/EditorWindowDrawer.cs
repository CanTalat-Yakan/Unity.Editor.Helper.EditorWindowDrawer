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

    public class EditorWindowDrawer : EditorWindow
    {
        public Rect Position => base.position;
        public Vector2 ScrollPosition;

        private string _desiredTitle;
        private Rect _desiredPosition;

        private static Vector2 s_minSize = new(256, 256);

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
            _bodyAction = null;
            _footerAction = null;
            _initialization = null;
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            _preProcessAction?.Invoke();
            EditorGUI.EndChangeCheck();

            if (_drawBorder)
                BeginDrawBorder();

            BeginWindow();

            BeginHeader(_headerSkin);
            _headerAction?.Invoke();
            EndHeader(_headerSkin);

            BeginBody(_bodySkin, ref ScrollPosition);
            _bodyAction?.Invoke();
            EndBody();

            BeginFooter(_footerSkin);
            _footerAction?.Invoke();
            EndFooter();

            EndWindow();

            if (_drawBorder)
                EndDrawBorder();

            _postProcessAction?.Invoke();
        }

        private static void BeginWindow() =>
            GUILayout.BeginVertical();

        private static void EndWindow() =>
            GUILayout.EndVertical();

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

        private static void BeginBody(EditorWindowStyle skin, ref Vector2 scrollPosition)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical(GetStyle(skin));

            if (skin == EditorWindowStyle.Window)
                GUILayout.Space(-17);
        }

        private static void EndBody()
        {
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
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

        private static readonly Color s_borderColorPro = new Color(0.11f, 0.11f, 0.11f);
        private static readonly Color s_borderColorLight = new Color(0.51f, 0.51f, 0.51f);
        private static readonly Color s_backgroundColorPro = new Color(0.22f, 0.22f, 0.22f);
        private static readonly Color s_backgroundColorLight = new Color(0.76f, 0.76f, 0.76f);
        private void BeginDrawBorder()
        {
            var borderColor = EditorGUIUtility.isProSkin ? s_borderColorPro : s_borderColorLight;
            var backgroundColor = EditorGUIUtility.isProSkin ? s_backgroundColorPro : s_backgroundColorLight;
            EditorGUI.DrawRect(new Rect(0, 0, Position.width, Position.height), borderColor);
            EditorGUI.DrawRect(new Rect(1, 1, Position.width - 2, Position.height - 2), backgroundColor);
            GUILayout.BeginArea(new Rect(1, 1, Position.width - 2, Position.height - 2));
        }

        private void EndDrawBorder() =>
            GUILayout.EndArea();

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

        public Vector2 GetLocalMousePosition()
        {
            // MouseInputFetcher.CurrentMousePosition is in screen coordinates.
            // base.position.position is the window's top-left in screen coordinates.
            return MouseInputFetcher.CurrentMousePosition - base.position.position;
        }

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