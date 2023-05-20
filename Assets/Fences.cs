using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fences : MonoBehaviour
{
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public float health = 100;
    public float threshold1 = 30;
    public float threshold2 = 60;
    
    public void AddHealth(float h)
    {
        this.health += h;
        if (this.health > this.threshold2)
        {
            this.Active(3);
        }
        else if (this.health > this.threshold1)
        {
            this.Active(2);
        }
    }

    public void RemoveHealth(float h)
    {
        this.health -= h;
        if (this.health < this.threshold1)
        {
            this.Active(1);
        }
        else if (this.health < this.threshold2)
        {
            this.Active(2);
        }
    }

    public void Active(int level)
    {
        if (this.level1.activeSelf)
        {
            this.level1.SetActive(false);
        }
        if (this.level2.activeSelf)
        {
            this.level2.SetActive(false);
        }
        if (this.level3.activeSelf)
        {
            this.level3.SetActive(false);
        }

        switch (level)
        {
            case 1:
                this.level1.SetActive(true);
                break;
            case 2:
                this.level2.SetActive(true);
                break;
            case 3:
                this.level3.SetActive(true);
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        this.level1 = this.transform.GetChild(0).gameObject;
        this.level2 = this.transform.GetChild(1).gameObject;
        this.level3 = this.transform.GetChild(2).gameObject;
    }
}
