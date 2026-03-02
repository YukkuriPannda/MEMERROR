using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class StartController : MonoBehaviour
{
    // Assign the Animator that controls the animation you want to play.
    public Animator animator;

    // Name of the trigger parameter in the Animator. Create this trigger in the Animator Controller.
    

    // Optional: call this from a Button's OnClick if you prefer OnClick over pointer events.
    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ActiveButton()
    {
        animator.SetTrigger("active");
    }
    public void DeactiveButton()
    {
        animator.SetTrigger("deactive");
    }
}
