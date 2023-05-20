using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    private static FPSDisplay instance;
    public static FPSDisplay Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("[FPSDisplay]");
                instance = go.AddComponent<FPSDisplay>();
                GameObject.DontDestroyOnLoad(go);
                go.hideFlags = HideFlags.HideAndDontSave;
            }
            return instance;
        }
    }

    public void OnInit()
    {

    }


    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        //new Color (0.0f, 0.0f, 0.5f, 1.0f);
        style.normal.textColor = Color.white;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        //string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        string text = string.Format("(fps:{0:0.})", fps);
        GUI.Label(rect, text, style);
    }
}
