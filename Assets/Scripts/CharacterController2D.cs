using System.Collections;
using UnityEngine;
public class CharacterController2D : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private const float movementSpeed = 10f;
    [SerializeField] private const float jumpForce = 19f;
    [SerializeField] private LayerMask groundLayerMask;   //what is Ground?
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private bool canAirControl = true;
    [SerializeField] private const float groundedRadius = 0.2f;
    [SerializeField] private PhysicsMaterial2D material;
    #endregion

    #region Audio
    public AudioSource source;
    public AudioClip clip;
    public AudioClip landClip; 
    #endregion

    private Transform currentMovingGround;
    private Rigidbody2D rb;

    public LayerMask wallLayerMask; // Layer, die als W�nde betrachtet werden sollen

    private Vector3 aboveGroundPos;
    private Vector2 velocity = Vector2.zero;

    private bool isGrounded;
    private bool aboveGround;
    private bool facingRight = true; //looking left/right
    private bool jumping;
    private bool wallDir;
    private bool isOnWall = false;

    private const float movementSmoothing = 0.05f;
    private float jumpingTime;
    public float wallCheckDistance = 0.5f; // Entfernung, in der nach W�nden gesucht wird



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        CheckIfOnWall();
        MoveWithMovingGround();
        checkGroundDistance();
    }

    private void MoveWithMovingGround()
    {
        if (currentMovingGround != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x + currentMovingGround.GetComponent<Rigidbody2D>().linearVelocity.x, rb.linearVelocity.y);
        }
    }


    public void Move(float moveInput, bool jumpInput)
    {
        if (isGrounded || canAirControl)
        {
            HandleMovement(moveInput);
            HandleRotation(moveInput);
        }
        if (isGrounded)
        {
            jumpingTime = 0f;
        }

        if (aboveGround && jumping && !isGrounded)
        {
            jumping = false;
            if (jumpingTime > 0.67f)
            {
                source.PlayOneShot(landClip);
            }
            jumpingTime = 0f;
        }

        if ((isGrounded && jumpInput) || (isOnWall && jumpInput))
        {
            if (rb.linearVelocity.y <= 0)
            {
                source.PlayOneShot(clip);
                isGrounded = false;
                StartJump();
                StartCoroutine(LandingSoundDelay());
            }
        }
    }
    private void checkGroundDistance()
    {
        aboveGroundPos = new Vector3(groundCheckPoint.position.x, groundCheckPoint.position.y - 1f, groundCheckPoint.position.z);
        aboveGround = Physics2D.OverlapCircle(aboveGroundPos, groundedRadius, groundLayerMask);
    }
    private void StartJump()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private IEnumerator LandingSoundDelay()
    {
        yield return new WaitForSeconds(0.2f);

        jumping = true;
        StartCoroutine(TrackJumpingTime());

    }

    private IEnumerator TrackJumpingTime()
    {
        while (jumping)
        {
            jumpingTime += Time.deltaTime; // Springzeit z�hlen, solange jumping == true ist
            yield return null;
        }
    }
    private void HandleMovement(float moveInput)
    {
        if (isOnWall)
        {
            if (!(wallDir && moveInput > 0))
            {
                rb.linearVelocity = new Vector2(moveInput * movementSpeed, rb.linearVelocity.y);

            }
            if (!(!wallDir && moveInput < 0))
            {
                rb.linearVelocity = new Vector2(moveInput * movementSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * movementSpeed, rb.linearVelocity.y);
        }
    }

    private void HandleRotation(float moveInput)
    {
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }
    public void HandleLadderMovementAndRotation(float moveInputH, float moveInputV, bool jumpInput)
    {
        jumpingTime = 0f;
        Vector2 targetVelocity = new Vector2(moveInputH * movementSpeed, moveInputV * movementSpeed);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, movementSmoothing);
        if (jumpInput)
        {
            source.PlayOneShot(clip);
            StartJump();
        }
        HandleRotation(moveInputH);
    }

    private void Flip() //flip facing left/right
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundedRadius, groundLayerMask);

    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("flyBarrel"))
        {
            if (currentMovingGround == null)
            {
                currentMovingGround = collision.transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("flyBarrel"))
        {
            currentMovingGround = null;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)   //Teleporter
    {
        if (collision.gameObject.CompareTag("worldBorderLvl1"))
        {
            transform.position = new Vector3(-13, -2, 0);
        }
        if (collision.gameObject.CompareTag("worldBorderLvl2"))
        {
            transform.position = new Vector3(62.5f, 15.5f, 0);
        }
        if (collision.gameObject.CompareTag("worldBorderLvl3"))
        {
            transform.position = new Vector3(146, 20.5f, 0);
        }
        if (collision.gameObject.CompareTag("teleporter"))
        {
            transform.position = new Vector3(-63, 22, 0);
        }
        if (collision.gameObject.CompareTag("teleporter2"))
        {
            transform.position = new Vector3(-10f, 0, 0);
        }

    }


    private void CheckIfOnWall()
    {
        // Raycast nach links
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayerMask);

        // Raycast nach rechts
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayerMask);

        if (hitLeft.collider != null)
        {
            wallDir = false;
        }
        if (hitRight.collider != null)
        {
            wallDir = true;
        }
        // Setze isOnWall, wenn entweder der linke oder rechte Raycast eine Wand trifft
        isOnWall = hitLeft.collider != null || hitRight.collider != null;
        if (jumping && isOnWall)
        {
            material.friction = 0f;
        }
        if (rb.linearVelocity.y < 0)
        {
            material.friction = 0.015f;
        }
    }
}