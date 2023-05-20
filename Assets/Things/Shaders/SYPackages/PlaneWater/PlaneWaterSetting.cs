using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWaterSetting : MonoBehaviour
{
    public bool boxProjection;
    public Vector3 center;
    public Vector3 size = Vector3.one;

    [Space(10)]
    public Vector2 maskTiling = Vector2.one;
    public Vector2 maskOffset;

    [Space(10)]
    [Range(0, 1)] public float edge = 1;
    [Range(0, 1)] public float range = 0;

    [HideInInspector] public Renderer m_renderer;

    private void OnEnable()
    {
        if(!m_renderer)
        m_renderer = GetComponent<Renderer>();

        if(!m_renderer) return;

        if(!m_renderer.sharedMaterial.enableInstancing)
            m_renderer.sharedMaterial.enableInstancing = true;

        Set();
    }

    public void Set()
    {
        if(!m_renderer) return;

        if(boxProjection)
            m_renderer.sharedMaterial.EnableKeyword("_BOXPROJECTION_ON");
        else
            m_renderer.sharedMaterial.DisableKeyword("_BOXPROJECTION_ON");

        Vector3 pos = transform.position + center;

        Vector3 min = pos - size * 0.5f;
        Vector3 max = pos + size * 0.5f;

        Vector4 c = pos;
        c.w = 1;

        MaterialPropertyBlock block = new MaterialPropertyBlock();

        block.SetVector("_Mask_ScaleOffset", new Vector4(maskTiling.x, maskTiling.y, maskOffset.x, maskOffset.y));
        block.SetVector("_WaterValue", new Vector4(edge, range, 0, 0));

        block.SetVector("_BoxCenter", c);
        block.SetVector("_BoxMin", min);
        block.SetVector("_BoxMax", max);

        m_renderer.SetPropertyBlock(block);
    }

    private void OnValidate()
    {
        Set();
    }

    private void Reset()
    {
        m_renderer = GetComponent<Renderer>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + center, 0.1f);

        Vector3 temp = size * 0.5f;

        Vector3 b0 = -temp + transform.position + center;
        Vector3 b1 = new Vector3(-temp.x, -temp.y, temp.z) + transform.position + center;
        Vector3 b2 = new Vector3(temp.x, -temp.y, temp.z) + transform.position + center;
        Vector3 b3 = new Vector3(temp.x, -temp.y, -temp.z) + transform.position + center;

        Gizmos.DrawLine(b0, b1);
        Gizmos.DrawLine(b1, b2);
        Gizmos.DrawLine(b2, b3);
        Gizmos.DrawLine(b3, b0);

        Vector3 t0 = new Vector3(-temp.x, temp.y, -temp.z) + transform.position + center;
        Vector3 t1 = new Vector3(-temp.x, temp.y, temp.z) + transform.position + center;
        Vector3 t2 = temp + transform.position + center;
        Vector3 t3 = new Vector3(temp.x, temp.y, -temp.z) + transform.position + center;

        Gizmos.DrawLine(t0, t1);
        Gizmos.DrawLine(t1, t2);
        Gizmos.DrawLine(t2, t3);
        Gizmos.DrawLine(t3, t0);

        Gizmos.DrawLine(t0, b0);
        Gizmos.DrawLine(t1, b1);
        Gizmos.DrawLine(t2, b2);
        Gizmos.DrawLine(t3, b3);
    }
}
