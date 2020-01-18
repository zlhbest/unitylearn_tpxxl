using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    //填充时间
    public float fillTime;
    public int xColumn;//列数
    public int yRow;//行数
    //甜品的种类
    public enum SweetsType
    {
        EMPTY,//空
        BARRIER,//障碍
        NORMAL,//普通类型
        ROW_CLEAR,//行消除
        COLUMN_CLEAR,//列消除
        RAINBOWCANDY,//糖果
        COUNT//标记
    }
    //甜品预制体的字典，我们可以通过甜品的中很累来得到对应的甜品游戏物体
    public Dictionary<SweetsType, GameObject> sweetPrefabDict;
    [System.Serializable]//这里用于显示在unity界面上进行赋值
    public struct SweetPrefab
    {
        public SweetsType type;
        public GameObject prefab;
    }
    public SweetPrefab[] sweetPrefabs;
    //巧克力预制体
    public GameObject gridPrefab;

    //二维数组定义，用于获取甜品
    private GameSweet[,] sweets;
    //将构造器私有，无法通过new方式实例化
    private GameManager() { }
    private void Awake()
    {
        Instance = new GameManager();
    }

    //要交换的两个甜品对象
    private GameSweet pressedSweet;

    private GameSweet enteredSweet;
    //所有的东西都是在gamemanager里面生成的，所以需要在gamemanager里面进行管理
    private void Start()
    {
        CreatPrefabDics();//创建甜品预制体字典
        InitBackGround();//创建背景巧克力网格
        Creatsweets();//创建甜品
        Destroy(sweets[4, 4].gameObject);
        CreatNewSweet(4, 4, SweetsType.BARRIER);
        StartCoroutine(AllFill());
    }
    private void Creatsweets()
    {
        sweets = new GameSweet[xColumn, yRow];
        for (int xcolumn = 0; xcolumn < xColumn; xcolumn++)
        {
            for (int yrow = 0; yrow < yRow; yrow++)
            {
                CreatNewSweet(xcolumn, yrow, SweetsType.EMPTY);
            }
        }
    }
    private void CreatPrefabDics()
    {
        sweetPrefabDict = new Dictionary<SweetsType, GameObject>();
        for (int i = 0; i < sweetPrefabs.Length; i++)
        {
            if (!sweetPrefabDict.ContainsKey(sweetPrefabs[i].type))
            {
                sweetPrefabDict.Add(sweetPrefabs[i].type, sweetPrefabs[i].prefab);
            }
        }
    }
    /// <summary>
    /// 实例化巧克力背景
    /// </summary>
    private void InitBackGround()
    {
        for (int xcolumn = 0; xcolumn < xColumn; xcolumn++)
        {
            for (int yrow = 0; yrow < yRow; yrow++)
            {
                GameObject chocolate = Instantiate(gridPrefab, CoreectPostion(xcolumn, yrow), Quaternion.identity);//最后这个是加旋转
                chocolate.transform.SetParent(transform);//把GameManager设置为父物体
            }
        }
    }
    /// <summary>
    /// 纠正位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 CoreectPostion(int x, int y)
    {
        //实际需要实例化巧克力块的x位置=GameManager位置的x坐标-大网格长度的一半+行列对应的坐标
        return new Vector3(transform.position.x - xColumn / 2f + x, transform.position.y + yRow / 2f - y);
    }

    //产生甜品的方法
    public GameSweet CreatNewSweet(int x, int y, SweetsType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDict[type], CoreectPostion(x, y), Quaternion.identity);//没有旋转角度
        newSweet.transform.parent = transform;
        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, type);
        return sweets[x, y];
    }
    /// <summary>
    /// 全部填充
    /// </summary>
    public IEnumerator AllFill(bool needRefill = true)
    {
        while (needRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }

            //清楚所有匹配好的甜品
            needRefill = ClearAllMatchedSweet();
        }
    }
    /// <summary>
    /// 部分填充
    /// </summary>
    public bool Fill()
    {
        bool filledNotFinished = false;//本次填充是否完成
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                GameSweet sweet = sweets[x, y];//得到当前元素位置的甜品对象
                if (sweet.CanMove())//如果无法移动，则无法向下填充
                {
                    GameSweet sweetBelow = sweets[x, y + 1];
                    if (sweetBelow.Type == SweetsType.EMPTY)//垂直填充
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MoveComponent.Move(x, y + 1, fillTime);
                        sweets[x, y + 1] = sweet;
                        CreatNewSweet(x, y, SweetsType.EMPTY);
                        filledNotFinished = true;
                    }
                    else //斜向填充
                    {
                        for (int down = -1; down <= 1; down++) //-1代表左下方，0代表正下方，1代表右下方
                        {
                            if (down != 0)
                            {
                                int downx = x + down;

                                if (downx >= 0 && downx < xColumn)
                                {
                                    GameSweet downSweet = sweets[downx, y + 1];
                                    if (downSweet.Type == SweetsType.EMPTY)
                                    {
                                        bool canfill = true;//用于判断垂直填充是否能满足填充要求
                                        for (int abveY = y + 1; abveY >= 0; abveY--)
                                        {
                                            GameSweet sweetAbove = sweets[downx, abveY];
                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbove.CanMove() && sweetAbove.Type != SweetsType.EMPTY)
                                            {
                                                canfill = false;
                                                break;
                                            }
                                        }
                                        if (!canfill)
                                        {
                                            if (sweet.CanMove())
                                            {
                                                Destroy(downSweet.gameObject);
                                                sweet.MoveComponent.Move(downx, y + 1, fillTime);
                                                sweets[downx, y + 1] = sweet;
                                                CreatNewSweet(x, y, SweetsType.EMPTY);
                                                filledNotFinished = true;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
        //对最上边的填充
        for (int x = 0; x < xColumn; x++)
        {
            GameSweet sweet = sweets[x, 0];
            if (sweet.Type == SweetsType.EMPTY)
            {
                GameObject newSweet = Instantiate(sweetPrefabDict[SweetsType.NORMAL], CoreectPostion(x, -1), Quaternion.identity);
                newSweet.transform.parent = transform;

                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, -1, this, SweetsType.NORMAL);
                sweets[x, 0].MoveComponent.Move(x, 0, fillTime);
                sweets[x, 0].ColorComponent.SetColor((ColorSweet.ColorType)Random.Range(0, sweets[x, 0].ColorComponent.NumColors));
                filledNotFinished = true;
            }
        }
        return filledNotFinished;
    }

    /// <summary>
    ///甜品是否相邻
    /// </summary>
    /// <returns></returns>
    private bool IsFriend(GameSweet sweet1, GameSweet sweet2)
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }
    /// <summary>
    /// 交换两个甜品
    /// </summary>
    private void Exchange(GameSweet sweet1, GameSweet sweet2)
    {
        if (sweet1.CanMove() && sweet2.CanMove())
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;
            if(MatchSweets(sweet1, sweet2.X, sweet2.Y) !=null || MatchSweets(sweet2, sweet1.X, sweet1.Y) != null)
            {
                int tempx = sweet1.X;
                int tempy = sweet1.Y;

                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, fillTime);
                sweet2.MoveComponent.Move(tempx, tempy, fillTime);
               StartCoroutine(AllFill(ClearAllMatchedSweet()));
            }
            else
            {
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }
          
        }
    }
    public void PressSweet(GameSweet sweet)
    {
        pressedSweet = sweet;
    }

    public void EnterSweet(GameSweet sweet)
    {
        enteredSweet = sweet;
    }
    public void ReleaseSweet()
    {
        if(IsFriend(pressedSweet, enteredSweet))
        {
            Exchange(pressedSweet, enteredSweet);
        }
    }
    //匹配方法
    public List<GameSweet> MatchSweets(GameSweet sweet,int newX,int newY)
    {
        if (sweet.CanColor())
        {
            ColorSweet.ColorType color = sweet.ColorComponent.Color;
            List<GameSweet> matchRowSweets = new List<GameSweet>();
            List<GameSweet> matchLineSweets = new List<GameSweet>();
            List<GameSweet> finishedMatchingSweets = new List<GameSweet>();
            //行匹配
            matchRowSweets.Add(sweet);
            //i=0代表往左，i=1代表往右
            for (int i = 0; i < 2; i++)
            {
                for (int xDistance = 1; xDistance < xColumn; xDistance++)
                {
                    int x;//
                    if (i==0)//左
                    {
                        x = newX - xDistance;
                    }
                    else
                    {
                        x = newX + xDistance;
                    }
                    if(x<0|| x>=xColumn)
                    {
                        break;
                    }
                    if (sweets[x,newY].CanColor()&&sweets[x,newY].ColorComponent.Color == color)
                    {
                        matchRowSweets.Add(sweets[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (matchRowSweets.Count>=3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchRowSweets[i]);
                }
            }
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchRowSweets.Count>=3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    //行匹配列表中满足匹配条件的每个元素上下一次进行列遍历
                    for (int j = 0; j <=1; j++)
                    {
                        for (int yDistance = 1; yDistance < yRow; yDistance++)
                        {
                            int y;//
                            if (i == 0)//左
                            {
                                y = newY - yDistance;
                            }
                            else
                            {
                                y = newY + yDistance;
                            }
                            if (y < 0 || y >= yRow)
                            {
                                break;
                            }
                            if (sweets[matchRowSweets[i].X, y].CanColor() && sweets[matchRowSweets[i].X, y].ColorComponent.Color == color)
                            {
                                matchLineSweets.Add(sweets[matchRowSweets[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (matchLineSweets.Count < 2)
                    {
                        matchLineSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchLineSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchLineSweets[i]);
                        }
                        break;
                    }
                }
            }
            if(finishedMatchingSweets.Count>=3)
            {
                return finishedMatchingSweets;
            }
            matchRowSweets.Clear();
            matchLineSweets.Clear();
            finishedMatchingSweets.Clear();
            //列匹配
            matchLineSweets.Add(sweet);
            //i=0代表往左，i=1代表往右
            for (int i = 0; i < 2; i++)
            {
                for (int yDistance = 1; yDistance < yRow; yDistance++)
                {
                    int y;//
                    if (i == 0)//左
                    {
                        y = newY - yDistance;
                    }
                    else
                    {
                        y = newY + yDistance;
                    }
                    if (y < 0 || y >= yRow)
                    {
                        break;
                    }
                    if (sweets[newX, y].CanColor() && sweets[newX, y].ColorComponent.Color == color)
                    {
                        matchLineSweets.Add(sweets[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchLineSweets[i]);
                }
            }
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    //行匹配列表中满足匹配条件的每个元素上下一次进行列遍历
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int xDistance = 1; xDistance < xColumn; xDistance++)
                        {
                            int x;//
                            if (i == 0)//左
                            {
                                x = newX - xDistance;
                            }
                            else
                            {
                                x = newX + xDistance;
                            }
                            if (x < 0 || x >= xColumn)
                            {
                                break;
                            }
                            if (sweets[x, matchLineSweets[i].Y].CanColor() && sweets[x, matchLineSweets[i].Y].ColorComponent.Color == color)
                            {
                                matchRowSweets.Add(sweets[x, matchLineSweets[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (matchRowSweets.Count < 2)
                    {
                        matchRowSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchRowSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchRowSweets[i]);
                        }
                        break;
                    }
                }
            }
            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
        }

        return null;
    }
    //清楚方法
    public bool ClearSweet(int x,int y)
    {
        if(sweets[x,y].CanClear() &&!sweets[x,y].ClearComponent.IsClearing)
        {
            sweets[x, y].ClearComponent.Clear();
            CreatNewSweet(x, y, SweetsType.EMPTY);
            return true;
        }
        else
        {
            return false;
        }
    }
    //清楚全部完成匹配的甜品
    private bool ClearAllMatchedSweet()
    {
        bool needRefill = false;
        for (int y = 0; y < yRow; y++)
        {
            for (int x = 0; x < xColumn; x++)
            {
                if(sweets[x,y].CanClear())
                {
                   var matchList =  MatchSweets(sweets[x,y],x,y);
                    if(matchList!=null)
                    {
                        for (int i = 0; i < matchList.Count; i++)
                        {
                            if(ClearSweet(matchList[i].X, matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }
}
