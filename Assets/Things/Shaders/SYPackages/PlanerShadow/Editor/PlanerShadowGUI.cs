using UnityEditor;
using UnityEngine;

public class PlanerShadowGUI : ShaderGUI
{
    private Material _targetMaterial;
    private MaterialEditor _editor;
    private MaterialProperty[] _properties;
    private static readonly int LightDir = Shader.PropertyToID("_Dir");

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this._targetMaterial = editor.target as Material;
        this._editor = editor;
        this._properties = properties;
        
        if (this._targetMaterial != null)
        {
            Show();
        }
    }

    private void Show()
    {
        MaterialProperty shadowColor = FindProperty("_Color", _properties);
        MaterialProperty offset = FindProperty("_Height", _properties);
        MaterialProperty offsetPlane = FindProperty("_OffsetPlane", _properties);
        MaterialProperty alpha = FindProperty("_Alpha", _properties);
        
        _editor.ColorProperty(shadowColor, shadowColor.displayName);
        _editor.FloatProperty(offset, offset.displayName);
        _editor.RangeProperty(offsetPlane, offsetPlane.displayName);
        _editor.RangeProperty(alpha, alpha.displayName);
        
        ReCalLightDir();
    }

    private void ReCalLightDir()
    {
        MaterialProperty shadowDir = FindProperty("_ShadowDir", _properties);
        MaterialProperty shadowLen = FindProperty("_ShadowLen", _properties);
        
        if(shadowDir == null || shadowLen == null)
            return;
        
        // Quaternion r = Quaternion.Euler(0, 180, 0);
        // Matrix4x4 m = Matrix4x4.Rotate(r);

        _editor.RangeProperty(shadowDir, shadowDir.displayName);
        _editor.RangeProperty(shadowLen, shadowLen.displayName);

        var dir = (shadowDir.floatValue * 2) * Mathf.PI;
        var len = (1.0f - shadowLen.floatValue);
        
        var y = len;
        
        var other = 1 - len * len;
        
        var c = Mathf.Cos(dir);
        var s = Mathf.Sin(dir);
        
        var x = c * (other);
        var z = s * (other);
        
        Vector3 planer = new Vector3(x ,y, z).normalized;

        //_targetMaterial.SetMatrix(LightMatrix, m);
        _targetMaterial.SetVector(LightDir, planer);
    }
}
