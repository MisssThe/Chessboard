using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class cross : MonoBehaviour
{
    public Chessboard chessboard;

    public List<Chessboard.CheesePos> PosList;
    //public GameObject xx;
    
    // Start is called before the first frame update
    public void Test(float second)
    {
        this.Invoke("Remove",second);
        // for (int i = 0;i<=PosList.Count - 1 ;i++)
        // {
        //     Instantiate(xx);
        // }

    }

    public void Remove()
    {
        if (this.chessboard == null)
        {
            return;
        }
        //找一排go
        this.chessboard.RemoveCheeses(PosList);
        this.chessboard.RecoverCheeses(PosList);
        this.enabled = false;
    }
}
