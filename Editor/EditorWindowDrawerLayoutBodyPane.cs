#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
        private void DrawPane(EditorPaneStyle paneStyle, bool expandHeight = true)
        {
            bool isVerticalOrientation = paneStyle == EditorPaneStyle.Top || paneStyle == EditorPaneStyle.Bottom;
            if (isVerticalOrientation)
                BeginPaneVerticalLayout(_paneSkin, Position.width, expandHeight, SplitterPosition, ref PaneScrollPosition);
            else BeginPane(_paneSkin, Position.height, SplitterPosition, ref PaneScrollPosition);

            _paneAction?.Invoke();

            if (!ContextMenuHandled && _paneMenu != null && Event.current.type == EventType.ContextClick)
            {
                ContextMenuHandled = false;
                _paneMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                Event.current.Use();
            }

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
    }
}
#endif