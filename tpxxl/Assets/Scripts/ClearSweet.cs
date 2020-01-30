using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSweet : MonoBehaviour
{
    public AnimationClip clearAnimation;

    private bool isClearing;

    public AudioClip destoryAudio;

    public bool IsClearing { get => isClearing;}

    protected GameSweet sweet;

    public virtual void Clear()
    {
        isClearing = true;
        StartCoroutine(ClearCoroutione());
    }

    private IEnumerator ClearCoroutione()
    {
        Animator animator = GetComponent<Animator>();
        if(animator!=null)
        {
            animator.Play(clearAnimation.name);
            //玩家得分+1 播放声音
            AudioSource.PlayClipAtPoint(destoryAudio,transform.position);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }
}

