#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public partial class EditorWindowDrawer : EditorWindow
    {
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
            RemoveUpdate();
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

        private Action _paneAction;
        private GenericMenu _paneMenu;
        private EditorPaneStyle _paneStyle;
        private EditorWindowStyle _paneSkin;
        public EditorWindowDrawer SetPane(Action pane, EditorPaneStyle style = EditorPaneStyle.Left, EditorWindowStyle skin = EditorWindowStyle.None, GenericMenu genericMenu = null)
        {
            _paneAction = pane;
            _paneMenu = genericMenu;
            _paneStyle = style;
            _paneSkin = skin;
            return this;
        }

        private Action _bodyAction;
        private GenericMenu _bodyMenu;
        private EditorWindowStyle _bodySkin;
        public EditorWindowDrawer SetBody(Action body, EditorWindowStyle skin = EditorWindowStyle.None, GenericMenu genericMenu = null)
        {
            _bodyAction = body;
            _bodyMenu = genericMenu;
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
    }
}
#endif