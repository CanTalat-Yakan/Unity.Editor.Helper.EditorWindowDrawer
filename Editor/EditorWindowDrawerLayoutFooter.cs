#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
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
    }
}
#endif