using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5;
    void Update()
    {
        transform.position += new Vector3(speed * 0.001f,0,0);
    }

     private void OnTriggerEnter(Collider other) 
     {

         Debug.Log("发生碰撞了");
         Destroy(this.gameObject);
     }

}
