using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//有空缺时棋子从上方进入
public class Chessboard : MonoBehaviour
{
    public GameObject chessPrefab;
    public int width,height;
    public Vector2 distance;
    public Vector2 position;
    public GameObject obstacle;
    public float size;
    public List<Texture2D> chessesTexture;
    private Vector2 m_Threshold;
    public GameObject vfx;
    public AnimationClip star;
    public AnimationClip remove;
    public AnimationClip appear;
    public int health = 1;

    private float m_StarTime;
    private float m_RemoveTime;
    private float m_AppearTime;
    private class Chess
    {
        public static List<Texture2D> ChessTextures;
        public static GameObject Vfx;
        public Chess(Transform transform, int x, int y)
        {
            this.transform = transform;
            this.link = new HashSet<Chess>();
            this.Color = Random.Range(0, ChessTextures.Count);
            this.transform.gameObject.GetComponent<Renderer>().material.mainTexture = ChessTextures[this.Color];
            pos.z = -10;
            this.X = x;
            this.Y = y;
            IsNull = false;
        }

        public void Drop(string name)
        {
            if (this.IsLock)
            {
                return;
            }
            this.transform.GetComponent<Animation>().Play(name);
            // this.transform.gameObject.GetComponent<Renderer>().material.color = Color.red;
            this.link.Clear();
            IsNull = true;
            IsChange = true;
            Instantiate(Vfx,this.transform);
        }

        public bool IsLock = false;

        public void Active(string name)
        {
            this.Color = Random.Range(0, ChessTextures.Count);
            this.transform.gameObject.GetComponent<Renderer>().material.mainTexture = ChessTextures[this.Color];
            this.link.Clear();
            this.transform.GetComponent<Animation>().Play(name);
            // this.transform.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }

        public bool IsNull;
        public bool IsChange;

        public void UpdatePos()
        {
            pos = Camera.main.WorldToScreenPoint(transform.position);
        }
        public Vector3 Pos()
        {
            if (pos.z < 0)
            {
                pos = Camera.main.WorldToScreenPoint(transform.position);
            }

            return pos;
        }

        public bool Move(Vector3 move, Chess[][] chesses, int width, int height)
        {
            if (this.IsNull)
            {
                return false;
            }
            //相邻块也要移动
            Chess temp = null;
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            {
                if (move.x > 0)
                {
                    if (X >= (width - 1))
                        return false;
                    temp = chesses[this.X + 1][this.Y];
                }
                else
                {
                    if (X <= 0)
                        return false;
                    temp = chesses[this.X - 1][this.Y];
                }
            }
            else
            {
                if (move.y > 0)
                {
                    if (Y <= 0)
                        return false;
                    temp = chesses[this.X][this.Y - 1];
                }
                else
                {
                    if (Y >= (height - 1))
                        return false;
                    temp = chesses[this.X][this.Y + 1];
                }
            }

            if (temp.IsNull)
            {
                return false;
            }
            this.transform.position = Camera.main.ScreenToWorldPoint(this.Pos() + move);
            if (temp != this.next)
            {
                this.next?.Record();
            }

            this.next = temp;
            this.next.transform.position = Camera.main.ScreenToWorldPoint(this.next.Pos() - move);
            return true;
        }

        public void Record()
        {
            this.transform.position = Camera.main.ScreenToWorldPoint(this.Pos());
            if (this.next == null)
            {
                return;
            }
            this.next.transform.position = Camera.main.ScreenToWorldPoint(this.next.Pos());
            this.next = null;
        }

        public void SetIndex(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.transform.name = x + ":" + y;
        }
        public int X, Y;
        public int Color;
        public Transform transform;
        private Vector3 pos;
        public Chess next;
        public HashSet<Chess> link;
    }

    private Chess[][] chesses = null;
    // Start is called before the first frame update
    void Start()
    {
        Transform transform1;
        (transform1 = this.transform).SetParent(Camera.main.transform);
        transform1.localPosition = new Vector3(this.position.x, this.position.y, 10);

        chesses = new Chess[width][];
        Chess.ChessTextures = this.chessesTexture;
        Chess.Vfx = this.vfx;
        float stepX = width * 0.5f;
        float stepY = height * 0.5f;
        for (int x = 0; x < width; x++) 
        {
            chesses[x] = new Chess[height];
            for (int y = 0; y < height; y++)
            {
                var chess = Object.Instantiate(this.chessPrefab, this.transform, true);
                chess.transform.name = x + ":" + y;
                chess.transform.localPosition = new Vector3(((float)x - stepX) * distance.x, 0, ((float)y - stepY) * distance.y);
                chess.transform.GetComponent<Animation>().Play(this.star.name);
                chesses[x][y] = new Chess(chess.transform, x, y);
            }
        }

        this.enabled = false;
        //等两秒后正式开始检测
        this.m_StarTime = this.star.length;
        this.m_RemoveTime = this.appear.length;
        this.m_AppearTime = this.remove.length;
        this.UpdateChessboardSize();
        StartCoroutine(nameof(StartUpdate));
//        this.TryDrag();
    }

    IEnumerator StartUpdate()
    {
        yield return new WaitForSeconds(this.m_StarTime);
        this.enabled = true;
        this.TryCollect();
    }

    // Update is called once per frame
    private bool m_IsRecover = true;
    void Update()
    {
        //更新棋盘尺寸
        this.UpdateChessboardSize();
        //检测当前是否有可消除的棋子
        if (!drop)
        {
            TryDrag();
        }
        else if (m_IsRecover)
        {
            m_IsRecover = false;
            StartCoroutine(nameof(RecoverChess)); 
        }
    }

    private Vector4 m_Different;
    private void UpdateChessboardSize()
    {
        Transform transform1;
        (transform1 = this.transform).localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        transform1.localScale = new Vector3(this.size,1, this.size);
        if (this.chesses[1][0].IsNull || this.chesses[0][1].IsNull)
        {
            return;
        }
        this.m_Threshold = this.chesses[1][0].Pos() - this.chesses[0][1].Pos();
    }

    //拖动棋子
    private Chess m_Choose;
    private Vector3 m_OriMousePos;
    private bool m_IsClick;
    private Vector3 m_Direction;
    private int m_DirFrame;
    private Chess m_OldNext;

    private void TryDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Transform curTf = hit.transform;
                string[] names = curTf.name.Split(':');
                m_Choose = chesses[int.Parse(names[0])][int.Parse(names[1])];
                m_OriMousePos = Input.mousePosition;
                this.m_DirFrame = 0;
                m_IsClick = true;
                m_Direction.y = -1;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_IsClick == false)
            {
                m_Choose = null;
                return;
            }
            //没达到阈值则返回
            Vector3 curMousePos = Input.mousePosition;
            Vector3 mouseOffset = curMousePos - m_OriMousePos;
            if (m_Threshold.x > Mathf.Abs(mouseOffset.x) * m_Direction.x && m_Threshold.y > Mathf.Abs(mouseOffset.y) * m_Direction.z)
            {
                this.m_Choose.Record();
            }
            m_Choose = null;
            m_IsClick = false;
        }
        if (m_IsClick)
        {
            if (m_Choose != null)
            {
                if (m_DirFrame++ < 10)
                {
                    return;
                }
                //给10帧用以确定方向
                Vector3 curMousePos = Input.mousePosition;
                Vector3 mouseOffset = curMousePos - m_OriMousePos;
                
                if (m_Direction.y < 0)
                {
                    if (Mathf.Abs(mouseOffset.x) < 0.01f && Mathf.Abs(mouseOffset.y) < 0.01f)
                    {
                        return;
                    }

                    if (Mathf.Abs(mouseOffset.x) < Mathf.Abs(mouseOffset.y) * 1.5)
                    {
                        m_Direction = new Vector3(0, 1, 1);
                    }
                    else
                    {
                        m_Direction = new Vector3(1, 1, 0);
                    }
                }
                //限制为x或y
                mouseOffset.x *= m_Direction.x;
                mouseOffset.y *= m_Direction.z;

                //相邻块也要移动
                if (!m_Choose.Move(mouseOffset, this.chesses, this.width, this.height))
                    return;
                //达到阈值则视为完成拖动
                if (m_Threshold.x < Mathf.Abs(mouseOffset.x) || m_Threshold.y < Mathf.Abs(mouseOffset.y))
                {
                    Chess next = this.m_Choose.next;
                    this.m_Choose.Record();
                    (this.m_Choose.transform.position, next.transform.position) = (next.transform.position, this.m_Choose.transform.position);
                    this.m_Choose.UpdatePos();
                    next.UpdatePos();
                    (chesses[this.m_Choose.X][this.m_Choose.Y], chesses[next.X][next.Y]) = (chesses[next.X][next.Y], chesses[this.m_Choose.X][this.m_Choose.Y]);
                    int temp_x, temp_y;
                    temp_x = m_Choose.X;
                    temp_y = m_Choose.Y;
                    m_Choose.SetIndex(next.X,next.Y);
                    next.SetIndex(temp_x,temp_y);
                    this.m_IsClick = false;
                    
                    //进行一次收集
                    TryCollect();
                }
                // Vector3 curObjectScreenPos = oriObjectScreenPos + mouseOffset;
                // Vector3 curObjectWorldPos = Camera.main.ScreenToWorldPoint(curObjectScreenPos);
                // if (mouseOffset.x > 0.1f || mouseOffset.y > 0.1f)
                // {
                //     
                // } 
                // curTf.position = curObjectWorldPos;
            }
        }

        // if (Input.touchCount > 0)
        // {
        //     Touch touch = Input.GetTouch(0);
        //     switch (touch.phase)
        //     {
        //         case TouchPhase.Began:
        //             startPos = touch.position;
        //             break;
        //         case TouchPhase.Moved:
        //             direction = touch.position - startPos;
        //             break;
        //         case TouchPhase.Ended:
        //             this.UpdateChessboard();
        //             break;
        //     }
        // }
    }


    //收集相同棋子，从左上往右下查找
    private void TryCollect()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (chesses[x][y].IsNull)
                {
                    continue;
                }

                if (x > 0)
                {
                    if (!chesses[x - 1][y].IsNull)
                    {
                        if (chesses[x - 1][y].Color == chesses[x][y].Color)
                        {
                            chesses[x][y].link.Add(chesses[x - 1][y]);
                            foreach (var link in chesses[x - 1][y].link)
                            {
                                chesses[x][y].link.Add(link);
                            }
                        }
                    }
                }

                if (x < (width - 1))
                {
                    if (!chesses[x + 1][y].IsNull)
                    {
                        if (chesses[x + 1][y].Color == chesses[x][y].Color)
                        {
                            chesses[x][y].link.Add(chesses[x + 1][y]);
                            foreach (var link in chesses[x + 1][y].link)
                            {
                                chesses[x][y].link.Add(link);
                            }
                        }
                    }
                }

                if (y > 0)
                {
                    if (!chesses[x][y - 1].IsNull)
                    {
                        if (chesses[x][y - 1].Color == chesses[x][y].Color)
                        {
                            chesses[x][y].link.Add(chesses[x][y - 1]);
                            foreach (var link in chesses[x][y - 1].link)
                            {
                                chesses[x][y].link.Add(link);
                            }
                        }
                    }
                }

                if (y < height - 1)
                {
                    if (!chesses[x][y + 1].IsNull)
                    {
                        if (chesses[x][y + 1].Color == chesses[x][y].Color)
                        {
                            chesses[x][y].link.Add(chesses[x][y + 1]);
                            foreach (var link in chesses[x][y + 1].link)
                            {
                                chesses[x][y].link.Add(link);
                            }
                        }
                    }
                }
            }
        }

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = width - 1; x >= 0; x--)
            {
                if (chesses[x][y].IsNull)
                {
                    continue;
                }

                int size = chesses[x][y].link.Count;
                if (chesses[x][y].link.Count < 3)
                {
                    continue;
                }
                chesses[x][y].link.Remove(chesses[x][y]);
                foreach (var chess in chesses[x][y].link)
                {
                    chess.Drop(this.remove.name);
                }

                chesses[x][y].Drop(this.remove.name);
                drop = true;
                this.AddScore(size);
            }
        }

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = width - 1; x >= 0; x--)
            {
                chesses[x][y].link.Clear();
            }
        }
    }

    private bool drop = false;

    public void AddScore(int size)
    {
        Fences.AddHealth(size * this.health);
    }

    //补充棋子
    IEnumerator RecoverChess()
    {
        yield return new WaitForSeconds(this.m_RemoveTime);
        //更新棋子
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = width - 1; x >= 0; x--)
            {
                if (chesses[x][y].IsNull && chesses[x][y].IsChange)
                {
                    chesses[x][y].Active(this.appear.name);
                    chesses[x][y].IsChange = false;
                    chesses[x][y].IsNull = false;
                }
            }
        }
        yield return new WaitForSeconds(this.m_AppearTime);
        drop = false;
        m_IsRecover = true;
        this.TryCollect();
    }

    [System.Serializable]
    public struct CheesePos
    {
        public int X, Y;
    }
    
    //恢复某些棋子的位置
    public void RecoverCheeses(List<CheesePos> names)
    {
        foreach (var n in names)
        {
            m_Choose = chesses[n.X][n.Y];
            m_Choose.Record();
        }
    }
    //消除某些棋子
    public void RemoveCheeses(List<CheesePos> names)
    {
        foreach (var n in names)
        {
            m_Choose = chesses[n.X][n.Y];
            m_Choose.Drop(this.remove.name);
            
        }
        this.AddScore(names.Count);
        this.drop = true;
    }

    public void Lock(List<CheesePos> names, bool flag)
    {
        foreach (var n in names)
        {
            m_Choose = chesses[n.X][n.Y];
            m_Choose.IsNull = flag;
            m_Choose.IsLock = flag;
            m_Choose.transform.GetComponent<MeshRenderer>().enabled = !flag;
        }
        if(this.obstacle == null)
            return;
        this.obstacle.SetActive(flag);
    }
}
