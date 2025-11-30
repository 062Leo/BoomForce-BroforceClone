using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    private Animator animator;
    private Rigidbody2D rb;
    private UDMapManager udMapManager;

    public float runSpeed = 40f;
    public float climbspeed = 40f;
    public float horizontalMove = 0f;
    public float verticalMove = 0f;

    public bool jump = false;
    private bool onLadder;
    private bool onLadderTop;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        udMapManager = FindObjectOfType<UDMapManager>();
    }
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        if (Input.GetButtonDown("Jump") && !onLadder || Input.GetButtonDown("Jump") && onLadderTop)
        {
            jump = true;
        }
        if (onLadderTop)
        {

            if (Input.GetKey(KeyCode.S))
            {
                verticalMove = -1 * climbspeed; // Bewegung nach unten
            }
            else { verticalMove = 0; }
        }
        else
        {
            verticalMove = Input.GetAxisRaw("Vertical") * climbspeed;
        }

    }
    void FixedUpdate()
    {
        // Move our character
        CheckIfOnLadder();

        if (!onLadder)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
            jump = false;
            rb.gravityScale = 4f;
        }
        else
        {
            if (onLadderTop)
            {
                controller.HandleLadderMovementAndRotation(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime, jump);
                jump = false;
            }
            controller.HandleLadderMovementAndRotation(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime, jump);
            rb.gravityScale = 0f;
        }
    }

    public void CheckIfOnLadder()
    {
        Vector2 pos = transform.position;
        pos.y = pos.y - 0.6f;
        Vector2 posJump = transform.position;
        posJump.y = posJump.y - 0.3f;
        if (udMapManager.getTileName(pos) == "Ladder" || udMapManager.getTileName(transform.position) == "Ladder")
        {
            if (!(udMapManager.getTileName(posJump) == "Ladder"))
            {
                onLadderTop = true;
            }
            else
            {
                onLadderTop = false;
            }
            onLadder = true;
        }
        else
        {
            onLadderTop = false;
            onLadder = false;
        }

    }
}
