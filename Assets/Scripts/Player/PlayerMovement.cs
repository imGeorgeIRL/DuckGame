using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public Transform cam;

    public Animator animator;

    public float moveSpeed;
    public float runSpeed;
    private bool isFlying;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    private float defaultSpeed;

    public CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        defaultSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); //A and D between -1 and 1
        float vertical = Input.GetAxisRaw("Vertical"); //W and S between -1 and 1

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //finds target angle for player facing
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smooths the transition between angles instead of staggered
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //applies the rotation

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime); //moving the player

            animator.SetInteger("AnimationPar", 1);
        }
        else
        {
            animator.SetInteger("AnimationPar", 0);
        }

       if (animator.GetInteger("AnimationPar") >= 0.1f)
        {
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Running", true);
                moveSpeed = runSpeed;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                animator.SetBool("Running", false);
                moveSpeed = defaultSpeed;
            }


        } 
    }
}
