#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace Examples.EditorWindow
{
    public class ExampleWindow
    {
        [MenuItem("Tools/Examples/Simple Example Window", priority = 10000)]
        public static void ShowWindow() =>
            new EditorWindowDrawer().ShowWindow("Example")
                .SetHeader(Header)
                .SetBody(Body, out _)
                .SetFooter(Footer);

        public static void Header() =>
            GUILayout.Label("Header");

        public static void Body() =>
            GUILayout.Label("Body");

        public static void Footer() =>
            GUILayout.Label("Footer");
    }
}
#endif