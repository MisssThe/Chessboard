// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class Chess : MonoBehaviour
// {
//     public AnimationClip star;
//     public AnimationClip remove;
//     public AnimationClip appear;
//
//     public GameObject chess;
//     public GameObject obstacle;
//
//     public IEnumerator Star()
//     {
//         if (this.chess == null)
//         {
//             return;
//         }
//         this.chess.transform.GetComponent<Animation>().Play(this.star.name);
//     }
//     public IEnumerator Appear()
//     {
//         if (this.chess == null)
//         {
//             return;
//         }
//         this.chess.transform.GetComponent<Animation>().Play(this.appear.name);
//     }
//
//     public IEnumerator Remove()
//     {
//         if (this.chess == null)
//         {
//             return;
//         }
//         this.chess.transform.GetComponent<Animation>().Play(this.remove.name);
//     }
//
//     public void Lock(bool flag)
//     {
//         if (this.obstacle == null)
//         {
//             return;
//         }
//
//         this.obstacle.SetActive(flag);
//         this.chess.SetActive(!flag);
//     }
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
// }
