#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class EditorWindowDrawer : EditorWindow
    {
        public EditorWindowDrawer EditorWindow;

        public enum GUISkin { None, Box, Window }

        private static Vector2 s_minSize = new(300, 400);
        private static Vector2 s_fallbackPosition = new(Screen.width / 2f, Screen.height / 2f);

        public EditorWindowDrawer(string title, Vector2? minSize = null, Vector2? position = null)
        {
            minSize ??= s_minSize;
            position ??= GetPosition(minSize);
            position ??= s_fallbackPosition;

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

        public EditorWindowDrawer ShowAsDropDown(Rect guiRect, Vector2? windowSize)
        {
            windowSize ??= minSize;
            base.ShowAsDropDown(GUIUtility.GUIToScreenRect(guiRect), windowSize.Value);
            base.Focus();
            return this;
        }

        public EditorWindowDrawer ShowModalUtility()
        {
            base.ShowModalUtility();
            return this;
        }

        private Vector2 GetPosition(Vector2? positionOffset = null)
        {
            var offset = Vector2.zero;
            if (positionOffset.HasValue)
            {
                offset = positionOffset.Value;
                offset /= 2;
                offset.x -= 30;
                offset.y -= 30;
            }
            return MouseInputFetcher.CurrentMousePosition - offset;
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
        private Vector2 _bodyScrollPosition;
        public EditorWindowDrawer SetBody(Action body, out Vector2 scrollPosition, GUISkin skin = GUISkin.None)
        {
            scrollPosition = _bodyScrollPosition;
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
    }
}
#endif