using System;
using UnityEngine;
namespace UnityEditor
{
    public class PBRShaderGUI : ShaderGUI
    {
        public enum RenderMode
        {
            Standard,//标准PBR
            SSSSkin,//SSS皮肤
            AnisotropicHair,//头发
        }
    }

}

