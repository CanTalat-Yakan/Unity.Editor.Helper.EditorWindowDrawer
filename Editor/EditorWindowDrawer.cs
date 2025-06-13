#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class EditorWindowDrawer : EditorWindow
    {
        public enum GUISkin { None, Box, Window }

        private static Vector2 s_minSize = new(300, 400);
        private static Vector2 s_fallbackPosition = new(Screen.width / 2f, Screen.height / 2f);

        private static bool DrawBorder;

        public EditorWindowDrawer(string title = null, Vector2? minSize = null, Vector2? position = null, bool centerPosition = true, bool drawBorder = false)
        {
            minSize ??= s_minSize;
            position ??= GetMousePosition(centerPosition, minSize);
            position ??= s_fallbackPosition;

            DrawBorder = drawBorder;

            if (string.IsNullOrEmpty(title))
                base.titleContent = new GUIContent(title);
            base.minSize = minSize.Value;
            base.position = new Rect(position.Value.x, position.Value.y, minSize.Value.x, minSize.Value.y);
        }

        public EditorWindowDrawer ShowWindow()
        {
            base.Show();
            return this;
        }

        public EditorWindowDrawer ShowUtility()
        {
            base.ShowUtility();
            return this;
        }

        public EditorWindowDrawer ShowPopup()
        {
            base.ShowPopup();
            return this;
        }

        public EditorWindowDrawer ShowAsDropDown(Rect rect, Vector2? size)
        {
            size ??= minSize;
            base.ShowAsDropDown(GUIUtility.GUIToScreenRect(rect), size.Value);
            base.Focus();
            return this;
        }

        public EditorWindowDrawer ShowModalUtility()
        {
            base.ShowModalUtility();
            return this;
        }

        private Vector2 GetMousePosition(bool centerPosition = true, Vector2? positionOffset = null)
        {
            var offset = Vector2.zero;
            
            return MouseInputFetcher.CurrentMousePosition - offset;
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

        public EditorWindowDrawer GetWindowPosition(ref Rect windowPosition)
        {
            windowPosition = base.position;
            return this;
        }

        private Vector2 _bodyScrollPosition;
        public EditorWindowDrawer GetBodyScrollPosition(ref Vector2 bodyScrollPosition)
        {
            bodyScrollPosition = _bodyScrollPosition;
            return this;
        }

        public EditorWindowDrawer GetCloseEvent(out Action closeEvent)
        {
            closeEvent = Close;
            return this;
        }

        public EditorWindowDrawer GetRepaintEvent(out Action repaintEvent)
        {
            repaintEvent = Repaint;
            return this;
        }


        public void OnLostFocus() =>
            Close();

        private void OnGUI()
        {
            _preProcessAction?.Invoke();

            if (DrawBorder)
                BeginDrawBorder();

            BeginWindow();

            BeginHeader(_headerSkin);
            _headerAction?.Invoke();
            EndHeader();

            BeginBody(_bodySkin, ref _bodyScrollPosition);
            _bodyAction?.Invoke();
            EndBody();

            BeginFooter(_footerSkin);
            _footerAction?.Invoke();
            EndFooter();

            EndWindow();

            if (DrawBorder)
                EndDrawBorder();

            _postProcessAction?.Invoke();
        }

        private static void BeginWindow() =>
            GUILayout.BeginVertical();

        private static void EndWindow() =>
            GUILayout.EndVertical();

        private static void BeginHeader(GUISkin skin) =>
            GUILayout.BeginVertical(skin switch
            {
                GUISkin.None => GUIStyle.none,
                GUISkin.Box => GUI.skin.box,
                GUISkin.Window => GUI.skin.window,
            });

        private static void EndHeader() =>
            GUILayout.EndVertical();

        private static void BeginBody(GUISkin skin, ref Vector2 scrollPosition)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical(skin switch
            {
                GUISkin.None => GUIStyle.none,
                GUISkin.Box => GUI.skin.box,
                GUISkin.Window => GUI.skin.window,
            });
            if (skin == GUISkin.Window)
                GUILayout.Space(-18);
        }

        private static void EndBody()
        {
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private static void BeginFooter(GUISkin skin) =>
            GUILayout.BeginVertical(skin switch
            {
                GUISkin.None => GUIStyle.none,
                GUISkin.Box => GUI.skin.box,
                GUISkin.Window => GUI.skin.window,
            });

        private static void EndFooter() =>
            GUILayout.EndVertical();

        private static readonly Color s_borderColorPro = new Color(0.11f, 0.11f, 0.11f);
        private static readonly Color s_borderColorLight = new Color(0.51f, 0.51f, 0.51f);
        private static readonly Color s_backgroundColorPro = new Color(0.22f, 0.22f, 0.22f);
        private static readonly Color s_backgroundColorLight = new Color(0.76f, 0.76f, 0.76f);
        private void BeginDrawBorder()
        {
            var borderColor = EditorGUIUtility.isProSkin ? s_borderColorPro : s_borderColorLight;
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), borderColor);

            var backgroundColor = EditorGUIUtility.isProSkin ? s_backgroundColorPro : s_backgroundColorLight;
            EditorGUI.DrawRect(new Rect(1, 1, position.width - 2, position.height - 2), backgroundColor);

            GUILayout.BeginArea(new Rect(1, 1, position.width - 2, position.height - 2));
        }
        private void EndDrawBorder() =>
            GUILayout.EndArea();
    }
}
#endif