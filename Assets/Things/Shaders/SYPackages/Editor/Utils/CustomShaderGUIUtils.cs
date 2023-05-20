
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngineInternal;
using Object = UnityEngine.Object;

namespace CustomShaderEditor
{
    public static class CustomShaderGUIUtils
    {
        private static Texture2D s_InfoIcon;
        private static Texture2D s_WarningIcon;
        private static Texture2D s_ErrorIcon;

        static GUIContent s_TextImage = new GUIContent();

        internal static Texture2D infoIcon
        {
            get
            {
                if (s_InfoIcon == null)
                    s_InfoIcon = EditorGUIUtility.FindTexture("console.infoicon");
                return s_InfoIcon;
            }
        }
        internal static Texture2D warningIcon
        {
            get
            {
                if (s_WarningIcon == null)
                    s_WarningIcon = EditorGUIUtility.FindTexture("console.warnicon");
                return s_WarningIcon;
            }
        }
        internal static Texture2D errorIcon
        {
            get
            {
                if (s_ErrorIcon == null)
                    s_ErrorIcon = EditorGUIUtility.FindTexture("console.erroricon");
                return s_ErrorIcon;
            }
        }

        internal static GUIContent TempContent(string t, Texture i)
        {
            s_TextImage.image = i;
            s_TextImage.text = t;
            return s_TextImage;
        }
        internal static Texture2D GetHelpIcon(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    return infoIcon;
                case MessageType.Warning:
                    return warningIcon;
                case MessageType.Error:
                    return errorIcon;
            }
            return null;
        }
        public static Rect DrawTitle(string title)
        {
            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 15; // 默认 9
            var rect = GUILayoutUtility.GetRect(0, 22);
            style.contentOffset = new Vector2(-1, -2);
            GUI.Box(rect, title, style);
            return rect;
        }
        public static Rect DrawShurikenLabel(string title, int fontsize)
        {
            var style = new GUIStyle("ShurikenLabel");
            style.alignment = TextAnchor.MiddleLeft;
            style.fontSize = fontsize; // 默认 9
            style.contentOffset = new Vector2(8, -fontsize);
            style.fontStyle = FontStyle.Bold;
            style.overflow = new RectOffset(0, 0, -fontsize, 0);
            style.normal.textColor = Color.white;
            var rect = GUILayoutUtility.GetRect(0, fontsize + 1);
            EditorGUI.LabelField(rect, title, style);
            //return rect;
            return rect;
        }
        public static bool Foldout(bool display, string title)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.alignment = TextAnchor.MiddleLeft;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);
            var rect = GUILayoutUtility.GetRect(16f, 24f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }
            return display;
        }
    }
}