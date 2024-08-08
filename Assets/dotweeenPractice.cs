using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class dotweeenPractice : MonoBehaviour
{
    public GameObject player;
    public GameObject target;

    Tween t;

    void Start()
    {
        t = player.transform.DOMove(target.transform.position, 2f, false);
        t.SetEase(Ease.InCubic).OnComplete(() =>
        {
            player.transform.DOJump(target.transform.position, 3, 1, 1f);
        });
    }
}
