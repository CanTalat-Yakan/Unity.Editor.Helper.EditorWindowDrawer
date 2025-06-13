#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class EditorWindowDrawer : EditorWindow
    {
        public enum GUISkin { None, Box, Window }

        public Rect Position => base.position;
        public Vector2 ScrollPosition;

        private Rect desiredPosition;
        private static Vector2 s_minSize = new(300, 400);

        public EditorWindowDrawer()
        {
            var minSize = s_minSize;
            var position = GetMousePosition(true, minSize);
            desiredPosition = new Rect(position.x, position.y, minSize.x, minSize.y);
        }

        public EditorWindowDrawer(string title = null, Vector2? minSize = null, Vector2? position = null, bool centerPosition = true)
        {
            minSize ??= s_minSize;
            position ??= GetMousePosition(centerPosition, minSize);

            if (string.IsNullOrEmpty(title))
                base.titleContent = new GUIContent(title);
            base.minSize = minSize.Value;
            desiredPosition = new Rect(position.Value.x, position.Value.y, minSize.Value.x, minSize.Value.y);
        }

        public EditorWindowDrawer ShowWindow()
        {
            base.Show();
            base.position = desiredPosition;
            return this;
        }

        public EditorWindowDrawer ShowUtility()
        {
            base.ShowUtility();
            base.position = desiredPosition;
            return this;
        }

        private bool _isUnfocusable = false;
        public EditorWindowDrawer ShowPopup()
        {
            base.ShowPopup();
            _isUnfocusable = true;
            return this;
        }

        public EditorWindowDrawer ShowAsDropDown(Rect rect, Vector2? size)
        {
            size ??= s_minSize;
            base.ShowAsDropDown(GUIUtility.GUIToScreenRect(rect), size.Value);
            base.Focus();
            _isUnfocusable = true;
            return this;
        }


        public bool s_drawBorder;
        public EditorWindowDrawer SetDrawBorder()
        {
            s_drawBorder = true;
            return this;
        }

        public Action s_preProcessAction;
        public EditorWindowDrawer SetPreProcess(Action preProcess)
        {
            s_preProcessAction = preProcess;
            return this;
        }

        public Action s_postProcessAction;
        public EditorWindowDrawer SetPostProcess(Action postProcess)
        {
            s_postProcessAction = postProcess;
            return this;
        }

        public Action s_headerAction;
        public GUISkin s_headerSkin;
        public EditorWindowDrawer SetHeader(Action header, GUISkin skin = GUISkin.None)
        {
            s_headerAction = header;
            s_headerSkin = skin;
            return this;
        }

        public Action s_bodyAction;
        public GUISkin s_bodySkin;
        public EditorWindowDrawer SetBody(Action body, GUISkin skin = GUISkin.None)
        {
            s_bodyAction = body;
            s_bodySkin = skin;
            return this;
        }

        public Action s_footerAction;
        public GUISkin s_footerSkin;
        public EditorWindowDrawer SetFooter(Action footer, GUISkin skin = GUISkin.None)
        {
            s_footerAction = footer;
            s_footerSkin = skin;
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

        public void OnLostFocus()
        {
            if (_isUnfocusable)
                Close();
        }

        private void OnGUI()
        {
            s_preProcessAction?.Invoke();

            if (s_drawBorder)
                BeginDrawBorder();

            BeginWindow();

            BeginHeader(s_headerSkin);
            s_headerAction?.Invoke();
            EndHeader();

            BeginBody(s_bodySkin, ref ScrollPosition);
            s_bodyAction?.Invoke();
            EndBody();

            BeginFooter(s_footerSkin);
            s_footerAction?.Invoke();
            EndFooter();

            EndWindow();

            if (s_drawBorder)
                EndDrawBorder();

            s_postProcessAction?.Invoke();
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
        private static readonly Color s_borderColor = EditorGUIUtility.isProSkin ? s_borderColorPro : s_borderColorLight;
        private static readonly Color s_backgroundColor = EditorGUIUtility.isProSkin ? s_backgroundColorPro : s_backgroundColorLight;
        private void BeginDrawBorder()
        {
            EditorGUI.DrawRect(new Rect(0, 0, Position.width, Position.height), s_borderColor);
            EditorGUI.DrawRect(new Rect(1, 1, Position.width - 2, Position.height - 2), s_backgroundColor);
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
    }
}
#endif