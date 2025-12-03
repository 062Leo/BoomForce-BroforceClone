using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    private Animator animator;
    private Rigidbody2D rb;
    private UDMapManager udMapManager;
    public GameObject mainMenu;
    public GameObject howToPlay;
    public GameObject exitCanvas;

    public float runSpeed = 40f;
    public float climbspeed = 40f;
    public float horizontalMove = 0f;
    public float verticalMove = 0f;

    public bool jump = false;
    private bool onLadder;
    private bool onLadderTop;
    [HideInInspector] public bool isPaused = false;
    private Vector2 savedVelocity;
    private float savedAngularVelocity;
    private bool wasKinematic;
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        mainMenu.SetActive(isPaused);
        

        if (isPaused)
        {
            // Save current physics state
            savedVelocity = rb.linearVelocity;
            savedAngularVelocity = rb.angularVelocity;
            wasKinematic = rb.isKinematic;
            
            // Freeze the character
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            
            // Optional: Pause audio
            AudioListener.pause = true;
        }
        else
        {
            howToPlay.SetActive(false);
            exitCanvas.SetActive(false);
            // Unfreeze the character
            rb.isKinematic = wasKinematic;
            rb.linearVelocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;
            
            // Optional: Resume audio
            AudioListener.pause = false;
        }
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        udMapManager = FindObjectOfType<UDMapManager>();
    }



    void Update()
    {
        // Toggle pause menu with Escape key
        if (SceneManager.GetActiveScene().name == "Game" && Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePause();
        }

        // Only process input if not paused
        if (!isPaused)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
            if ((Input.GetButtonDown("Jump") && !onLadder) || (Input.GetButtonDown("Jump") && onLadderTop))
            {
                jump = true;
            }
            
            if (onLadderTop)
            {
                verticalMove = Input.GetKey(KeyCode.S) ? -1 * climbspeed : 0f;
            }
            else
            {
                verticalMove = Input.GetAxisRaw("Vertical") * climbspeed;
            }
        }
        else
        {
            // Reset movement when paused
            horizontalMove = 0f;
            verticalMove = 0f;
            jump = false;
        }

    }
    void FixedUpdate()
    {
        // Skip physics if game is paused
        if (isPaused)
            return;
            
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
