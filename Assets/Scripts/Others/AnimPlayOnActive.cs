using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPlayOnActive : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (animator != null)
            animator.SetTrigger("Play");
    }
}
