using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//添加障碍物
public class Obstacle : MonoBehaviour
{
    public List<Chessboard.CheesePos> PosList;
    public Chessboard chessboard;
    public void Add()
    {
        if (this.chessboard == null)
        {
            return;
        }

        this.chessboard.Lock(this.PosList, true);
    }

    public void Remove()
    {
        if (this.chessboard == null)
        {
            return;
        }

        this.chessboard.Lock(this.PosList, false);
    }
    // Start is called before the first frame update
    void Start()
    {
        this.Add();
    }
}
