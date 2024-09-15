using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
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

    private float vSpeed = 0f; //current vertical velocity
    private float gravity = -9.81f;
    private float fallSpeedMultiplier = 2f;
    private float groundCheckDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask waterMask;

    private bool isGrounded;
    private bool isSwimming;

    [SerializeField] private float flightTime;
    [SerializeField] private float takeoffTime = 1.2f;
    [SerializeField] private float landingTime = 2.2f;

    public CharacterController controller;

    private Vector3 flightDirection;

    // Start is called before the first frame update
    void Start()
    {
        defaultSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {


        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        isSwimming = Physics.CheckSphere(transform.position, groundCheckDistance, waterMask);
        
        if (isSwimming)
        {
            animator.SetBool("onWater", true);
        }
        else
        {
            animator.SetBool("onWater", false);
        }

        if (isGrounded && vSpeed < 0)
        {
            vSpeed = -2f;
            
        }

        float horizontal = Input.GetAxisRaw("Horizontal"); //A and D between -1 and 1
        float vertical = Input.GetAxisRaw("Vertical"); //W and S between -1 and 1

        if (isFlying)
        {
            vertical = 1f;
            moveSpeed = runSpeed;
        }

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //finds target angle for player facing
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smooths the transition between angles instead of staggered
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //applies the rotation

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime); //moving the player

            animator.SetInteger("AnimationPar", 1);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(Flying());
            }
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

        if (!isGrounded && !isSwimming)
        {
            vSpeed += gravity * fallSpeedMultiplier * Time.deltaTime; //increase fall speed
        }
       
       //apply vertical movement aka fall...
       Vector3 velocity = new Vector3(0, vSpeed, 0);
       controller.Move(velocity * Time.deltaTime);

        
    }


    private IEnumerator Flying()
    {
        //float oldSpeed = moveSpeed;

        //moveSpeed = runSpeed;
        if (isSwimming)
        {
            animator.SetBool("takeOffFromWater", true);
        }

        isFlying = true;
        //flightDirection = transform.forward;

        //store the old gravity and movement state
        float oldGravity = gravity;
        animator.SetTrigger("takeOff");
        

        yield return new WaitForSeconds(1f); //wait a second for animation to start

        gravity = 0f; //disable gravity


        //define the starting position and target height
        Vector3 startingPosition = transform.position;
        Vector3 targetPosition = startingPosition + new Vector3(0f, 5f, 0f); //move up five units, can change

        float elapsedTime = 0f;

        while (elapsedTime < takeoffTime)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / takeoffTime); //lerp from start to target in 1.2 seconds (the length of takeoff anim)
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;

        //lock Y position when at target height
        Vector3 lockedFlightPosition = transform.position;
        lockedFlightPosition.y = targetPosition.y;

        elapsedTime = 0f;

        while (elapsedTime < flightTime) //while the flight is active, keep the player at the same Y level
        {
            Vector3 currentPos = transform.position;
            currentPos.y = lockedFlightPosition.y;
            transform.position = currentPos;

            elapsedTime += Time.deltaTime;
            yield return null; //Wait until next frame
        }
        //move character model up gradually (lerp probably)
        //yield return new WaitForSeconds(flightTime);

        //landing phase: gradually move down until ground is detected
        animator.SetTrigger("land");
        while (!isGrounded && !isSwimming)
        {
            Vector3 descent = new Vector3(0, -landingTime * Time.deltaTime, 0);
            controller.Move(descent);

            yield return null;
        }


        gravity = oldGravity;
        //moveSpeed = oldSpeed;
        isFlying = false;
        animator.SetBool("takeOffFromWater", false);
    }
}
