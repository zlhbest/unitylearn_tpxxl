using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweet : MonoBehaviour
{
    private GameSweet sweet;

    private IEnumerator moveCoroutine;//这样得到其他指令的时候我们可以终止掉这个协程

    private void Awake()
    {
        sweet = GetComponent<GameSweet>();//找到挂载到这个物体上的Gamesweet脚本
    }
    //开启或者结束这个协程
    public void Move(int newX,int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);//先将每一帧的关闭掉
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
        
    }
    //负责移动的协程
    private IEnumerator MoveCoroutine(int newX, int newY,float time)
    {
        sweet.X = newX;
        sweet.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = sweet.gameManager.CoreectPostion(newX, newY);

        for (float t = 0; t < time; t+=Time.deltaTime)
        {
            sweet.transform.position = Vector3.Lerp(startPos, endPos,t/time);
            yield return 0;//等待一帧
        }
        sweet.transform.position = endPos;//确保在突发情况，让甜品到该到的位置，这是一句确保代码
    }
}

