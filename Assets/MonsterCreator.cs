using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCreator : MonoBehaviour
{
    public GameObject monster;
    public float interval;
    public int MaxCount;
    public Vector2 offset;
    private float time;

    void Start()
    {
        if (this.monster == null)
        {
            this.enabled = false;
            return;
        }
    }
    void Update()
    {
        if ((time += Time.deltaTime) < this.interval)
        {
            return;
        }
        time = 0;
        this.Create();
    }

    private void Create()
    {
        int number = Random.Range(0, MaxCount);
        for (int index = 0; index < number; index++)
        {
            var mon = Instantiate(this.monster, this.transform);
            mon.transform.localPosition = new Vector3(0,Random.Range(0, 6) / this.offset.y + this.offset.x,0);
            int PosY = Random.Range(0, 6);
        }
    }
}
