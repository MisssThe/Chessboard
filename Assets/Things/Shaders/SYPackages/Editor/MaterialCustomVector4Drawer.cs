using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomShaderEditor
{

    public class MaterialCustomVector4Drawer : MaterialPropertyDrawer
    {
        private readonly string groupName;
        private readonly float[] mMin;
        private readonly float[] mMax;


        public MaterialCustomVector4Drawer(string groupName, string n1, string v1, string n2, string v2, string n3, string v3, string n4, string v4) : this(groupName, new[] { n1, n2, n3, n4 }, new[] { v1, v2, v3, v4 }) { }


        public MaterialCustomVector4Drawer(string groupName, string[] mins, string[] maxs)
        {
            this.groupName = groupName;
            this.mMin = new float[mins.Length];
            for (int i = 0; i < mins.Length; i++)
            {
                var min = mins[i];
                float f;
                char sign = min.ToCharArray()[0];
                string value = min.Substring(1);
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out f))
                {
                    this.mMin[i] = f * (sign == 'n' ? -1 : 1);
                }
            }
            this.mMax = new float[maxs.Length];
            for (int i = 0; i < maxs.Length; i++)
            {
                var max = maxs[i];
                float f;
                char sign = max.ToCharArray()[0];
                string value = max.Substring(1);
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out f))
                {
                    this.mMax[i] = f * (sign == 'n' ? -1 : 1);
                }
            }
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            bool show = editor.IsHeaderShowInInspector(groupName);
            if (show)
            {
                var descs = label.Split('|');

                CustomShaderGUIUtils.DrawShurikenLabel(descs[0],11);

                 var descX = "X ";
                var descY = "Y ";
                var descZ = "Z ";
                var descW = "W ";
                if (descs.Length > 1)
                    descX = descs[1] + " ";
                if (descs.Length > 2)
                    descY = descs[2] + " ";
                if (descs.Length > 3)
                    descZ = descs[3] + " ";
                if (descs.Length > 4)
                    descW = descs[4] + " ";

                EditorGUI.BeginChangeCheck();
                var val = prop.vectorValue;
                var x = EditorGUILayoutFloatSlider(descX, val.x, mMin[0], mMax[0]);
                var y = EditorGUILayoutFloatSlider(descY, val.y, mMin[1], mMax[1]);
                var z = EditorGUILayoutFloatSlider(descZ, val.z, mMin[2], mMax[2]);
                var w = EditorGUILayoutFloatSlider(descW, val.w, mMin[3], mMax[3]);

                if (EditorGUI.EndChangeCheck())
                {
                    prop.vectorValue = new Vector4(x, y, z,w);
                }
            }
            else
            {

            }

            ////val.x = EditorGUILayoutFloatSlider(descX, val.x, mMin[0], mMax[0]);
            ////val.y = EditorGUILayoutFloatSlider(descY, val.y, mMin[1], mMax[1]);
            ////val.z = EditorGUILayoutFloatSlider(descZ, val.z, mMin[2], mMax[2]);
            ////val.w = EditorGUILayoutFloatSlider(descW, val.w, mMin[3], mMax[3]);

            ////var descX = "X ";
            ////var descY = "Y ";
            ////var descZ = "Z ";
            ////var descW = "W ";
            ////if (descs.Length > 1)
            ////    descX = descs[1] + " ";
            ////if (descs.Length > 2)
            ////    descY = descs[2] + " ";
            ////if (descs.Length > 3)
            ////    descZ = descs[3] + " ";
            ////if (descs.Length > 4)
            ////    descW = descs[4] + " ";


            ////if (mLen >= 1)
            ////    val.x = EditorGUILayoutFloatSlider(descX, val.x, mMin[0], mMax[0]);
            ////if (mLen >= 2)
            ////    val.y = EditorGUILayoutFloatSlider(descY, val.y, mMin[1], mMax[1]);
            ////if (mLen >= 3)
            ////    val.z = EditorGUILayoutFloatSlider(descZ, val.z, mMin[2], mMax[2]);
            ////if (mLen >= 4)
            ////    val.w = EditorGUILayoutFloatSlider(descW, val.w, mMin[3], mMax[3]);

            ////prop.vectorValue = val;
        }

        private float EditorGUILayoutFloatSlider(string descX, float val, float min, float max)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(descX, GUILayout.ExpandWidth(false));
            val = GUILayout.HorizontalSlider(val, min, max, GUILayout.ExpandWidth(true));
            val = EditorGUILayout.FloatField(val, GUILayout.Width(64));
            EditorGUILayout.EndHorizontal();
            return val;
        }
    }
}