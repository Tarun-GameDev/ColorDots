using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleAnim : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] Animator animator;

    bool toggleinOn = false;

    private void Start()
    {
        if(toggle == null)
            toggle = GetComponent<Toggle>();

        Invoke("PlayToggleAnim", 1f);
    }


    public void PlayToggleAnim()
    {
        if(toggle != null && animator != null)
        {
            if (toggle.isOn)
            {
                animator.SetTrigger("ToggleOn");
                if (!toggleinOn)
                {

                    toggleinOn = true;
                }
            }
            else
            {
                animator.SetTrigger("ToggleOff");
                if (toggleinOn)
                {

                    toggleinOn = false;
                }
            }

        }
    }
}
