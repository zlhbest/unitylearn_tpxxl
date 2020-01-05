using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour
{
    private int x;
    public int X
    {
        get
        {
            return x;
        }
        set
        {
            if(CanMove())
            {
                x = value;
            }
        }
    }
    private int y;
    public int Y 
    {
        get
        {
            return y;
        }
        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }
    public GameManager.SweetsType Type { get; private set; }
    public MoveSweet MoveComponent { get; private set; }
    public ColorSweet ColorComponent { get ;private set ; }

    [HideInInspector]//在显示面板隐藏掉这个属性
    public GameManager gameManager;

    public void Init(int _x,int _y,GameManager _gameManager,GameManager.SweetsType _type)
    {
        X = _x;
        Y = _y;
        gameManager = _gameManager;
        Type = _type;
    }
    public bool CanMove()
    {
        return MoveComponent != null;//如果脚本是空的，那就不能移动，只有移动脚本不为空的时候才能移动
    }
    public bool CanColor()
    {
        return ColorComponent != null;//如果脚本是空的，那就不能移动，只有移动脚本不为空的时候才能移动
    }
    private void Awake()
    {
        MoveComponent = GetComponent<MoveSweet>();
        ColorComponent = GetComponent<ColorSweet>();
    }

    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }
    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
    }
    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }
}
