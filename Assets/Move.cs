using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5;
    public int attack = 1;
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
         Destroy(this.gameObject);
     }

}
