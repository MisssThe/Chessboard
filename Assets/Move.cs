using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5;
    public int attack = 1;
    public GameObject mesh;
    public GameObject vfx;
    public float deadTime;
    public string moveName;
    void Start()
    {
        this.GetComponent<Animation>().Play(moveName);
    }
    void Update()
    {
        transform.position += new Vector3(speed * 0.001f,0,0);
    }

     private void OnTriggerEnter(Collider other)
     {
         if (other.transform.gameObject.layer != 3)
         {
             return;
         }
         // if(other.transform.layer )
         Fences.RemoveHealth(this.attack);
         //播放死亡动画
         Invoke("Remove", this.deadTime);
         this.GetComponent<Animation>().Stop();
             // = false;
         this.vfx.SetActive(true);
         this.mesh.SetActive(false);
             // .enabled = false;
         this.enabled = false;
     }

     private void Remove()
     {
         Destroy(this.gameObject);
     }
}
