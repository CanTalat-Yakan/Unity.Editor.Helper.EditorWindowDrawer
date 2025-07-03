#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
        private void DrawBody(EditorPaneStyle? paneStyle = null, bool expandHeight = true)
        {
            bool isVerticalOrientation = paneStyle == EditorPaneStyle.Top || paneStyle == EditorPaneStyle.Bottom;
            if (isVerticalOrientation)
                BeginBodyVerticalLayout(_bodySkin, Position.width, expandHeight, SplitterPosition, ref BodyScrollPosition);
            else BeginBody(_bodySkin, Position.height, paneStyle == null, ref BodyScrollPosition);

            _bodyAction?.Invoke();

            if (!ContextMenuHandled && _bodyMenu != null && Event.current.type == EventType.ContextClick)
            {
                ContextMenuHandled = false;
                _bodyMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                Event.current.Use();
            }

            if (paneStyle != null)
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
    }
}
#endif