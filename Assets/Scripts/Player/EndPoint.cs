using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    public bool isFilled = false;

    [SerializeField] Animator animator;

    public void EndPointReached()
    {
        isFilled = true;
        if (animator != null)
            animator.SetTrigger("Reached");
    }
}
