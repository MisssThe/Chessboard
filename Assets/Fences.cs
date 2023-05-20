using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fences : MonoBehaviour
{
    public static GameObject level1;
    public static GameObject level2;
    public static GameObject level3;
    public float health = 20;
    public float threshold1 = 30;
    public float threshold2 = 60;
    public static float SM_health;
    public static float SM_threshold1 = 30;
    public static float SM_threshold2 = 60;
    
    public static void AddHealth(float h)
    {
        SM_health += h;
        if (SM_health > SM_threshold2)
        {
            Active(3);
        }
        else if (SM_health > SM_threshold1)
        {
            Active(2);
        }
    }

    public static void RemoveHealth(float h)
    {
        SM_health -= h;
        if (SM_health < 0)
        {
            Active(0);
        }
        else if (SM_health < SM_threshold1)
        {
            Active(1);
        }
        else if (SM_health < SM_threshold2)
        {
            Active(2);
        }
    }

    public static void Active(int level)
    {
        if (level1.activeSelf)
        {
            level1.SetActive(false);
        }
        if (level2.activeSelf)
        {
            level2.SetActive(false);
        }
        if (level3.activeSelf)
        {
            level3.SetActive(false);
        }

        switch (level)
        {
            case 0:
                level1.transform.parent.gameObject.SetActive(false);
                break;
            case 1:
                level1.SetActive(true);
                break;
            case 2:
                level2.SetActive(true);
                break;
            case 3:
                level3.SetActive(true);
                break;
        }
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        level1 = this.transform.GetChild(0).gameObject;
        level2 = this.transform.GetChild(1).gameObject;
        level3 = this.transform.GetChild(2).gameObject;
        SM_health = this.health;
        SM_threshold1 = this.threshold1;
        SM_threshold2 = this.threshold2;
        if (SM_health < SM_threshold1)
        {
            Active(1);
        }
        else if (SM_health < SM_threshold2)
        {
            Active(2);
        }
        else
        {
            Active(3);
        }
    }
}
