#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
        public float SplitterPosition = 300f;
        public float MinSplitterSize = 50;

        private const float SplitterSize = 5f;

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

            EditorGUI.DrawRect(visibleSplitter, DarkColor);

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
                    bool isToolbarHeaderActive = _headerAction != null && _headerSkin == EditorWindowStyle.Toolbar;
                    float headerOffset = isToolbarHeaderActive ? 21 : 0;
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
    }
}
#endif