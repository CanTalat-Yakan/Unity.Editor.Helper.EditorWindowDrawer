#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class EditorWindowDrawer : EditorWindow
    {
        public enum GUISkin
        {
            None,
            Margin,
            BigMargin,
            Box,
            Window,
            HelpBox,
            Toolbar
        }

        public Rect Position => base.position;
        public Vector2 ScrollPosition;

        private string _desiredTitle;
        private Rect _desiredPosition;

        private static Vector2 s_minSize = new(300, 400);

        public EditorWindowDrawer()
        {
            var size = s_minSize;
            var minSize = s_minSize;
            var position = GetMousePosition(true, minSize);
            _desiredPosition = new Rect(position.x, position.y, minSize.x, minSize.y);
        }

        public EditorWindowDrawer(string title = null, Vector2? minSize = null, Vector2? size = null, Vector2? position = null, bool centerPosition = true)
        {
            size ??= s_minSize;
            minSize ??= s_minSize;
            position ??= GetMousePosition(centerPosition, minSize);

            _desiredTitle = string.IsNullOrEmpty(title) ? string.Empty : title;
            _desiredPosition = new Rect(position.Value.x, position.Value.y, size.Value.x, size.Value.y);

            base.titleContent = new GUIContent(_desiredTitle);
            base.minSize = minSize.Value;
        }

        public EditorWindowDrawer ShowWindow()
        {
            base.Show();
            base.position = _desiredPosition;
            _initialization?.Invoke();
            return this;
        }

        public EditorWindowDrawer ShowUtility()
        {
            base.ShowUtility();
            base.position = _desiredPosition;
            base.titleContent = new GUIContent(_desiredTitle);
            _initialization?.Invoke();
            return this;
        }

        public EditorWindowDrawer ShowPopup()
        {
            base.ShowPopup();
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
        private GUISkin _headerSkin;
        public EditorWindowDrawer SetHeader(Action header, GUISkin skin = GUISkin.None)
        {
            _headerAction = header;
            _headerSkin = skin;
            return this;
        }

        private Action _bodyAction;
        private GUISkin _bodySkin;
        public EditorWindowDrawer SetBody(Action body, GUISkin skin = GUISkin.None)
        {
            _bodyAction = body;
            _bodySkin = skin;
            return this;
        }

        private Action _footerAction;
        private GUISkin _footerSkin;
        public EditorWindowDrawer SetFooter(Action footer, GUISkin skin = GUISkin.None)
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
        }

        private void OnGUI()
        {
            _preProcessAction?.Invoke();

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

        private static void BeginHeader(GUISkin skin)
        {
            if (skin == GUISkin.Toolbar)
                GUILayout.BeginHorizontal(GetStyle(skin));
            else GUILayout.BeginVertical(GetStyle(skin));

            if (skin == GUISkin.Window)
                GUILayout.Space(-17);
        }

        private static void EndHeader(GUISkin skin)
        {
            if (skin == GUISkin.Toolbar)
                GUILayout.EndHorizontal();
            else GUILayout.EndVertical();

            if (skin == GUISkin.HelpBox)
                GUILayout.Space(-2);
        }

        private static void BeginBody(GUISkin skin, ref Vector2 scrollPosition)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical(GetStyle(skin));

            if (skin == GUISkin.Window)
                GUILayout.Space(-17);
        }

        private static void EndBody()
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private static void BeginFooter(GUISkin skin)
        {
            if (skin == GUISkin.HelpBox)
                GUILayout.Space(-2);

            GUILayout.BeginVertical(GetStyle(skin));

            if (skin == GUISkin.Window)
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
                offset.x -= 30;
                offset.y -= 30;
            }
            return MouseInputFetcher.CurrentMousePosition - offset;
        }

        private static GUIStyle GetStyle(GUISkin skin)
        {
            return skin switch
            {
                GUISkin.Box => GUI.skin.box,
                GUISkin.Window => GUI.skin.window,
                GUISkin.HelpBox => EditorStyles.helpBox,
                GUISkin.Toolbar => EditorStyles.toolbar,
                GUISkin.Margin => EditorStyles.inspectorFullWidthMargins,
                GUISkin.BigMargin => EditorStyles.inspectorDefaultMargins,
                GUISkin.None or _ => GUIStyle.none,
            };
        }
    }
}
#endif