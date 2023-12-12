using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    { 
        bool isWalking = animator.GetBool("IsMoving");
        bool isThrowing = animator.GetBool("IsKicking");
        bool isPickUp = animator.GetBool("IsPickUp");

        bool forwardPressed = Input.GetKey("w");
        bool backPressed = Input.GetKey("s");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");

        if (Input.GetKeyDown("space"))
        {
            animator.SetBool("IsPickUp", true);
        }

        if (Input.GetKeyUp("space"))
        {
            GameManager.instance.kickCount += 1;
            animator.SetBool("IsPickUp", false);
            animator.SetBool("IsKicking", true);
        }

        //Walking
        if (!isWalking && (forwardPressed || backPressed || leftPressed || rightPressed))
        {
            animator.SetBool("IsPickUp", false);
            animator.SetBool("IsKicking", false);

            animator.SetBool("IsMoving", true);
        }
        else if (isWalking)
        {
            animator.SetBool("IsMoving", false);
        }

    }
}
