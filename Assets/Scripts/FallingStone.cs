using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingStone : MonoBehaviour
{
    private Tilemap map;
    public Animator animator;
    private GameObject childObject;
    private Rigidbody2D rb;
    private BoxCollider2D coll2d;
    private Vector3 startPos;
    private bool isFalling;
    private bool started;
    private bool isBurning;
    private float burn;
    private float count = 0;
    bool startAnim;

    void Awake()
    {
        startPos = transform.position;
        childObject = transform.GetChild(0).gameObject;
        map = GameObject.Find("Destroyable").GetComponent<Tilemap>();
        rb = GetComponent<Rigidbody2D>();
        isFalling = false;
        started = false;
        coll2d = GetComponent<BoxCollider2D>();
        isBurning = false;
        burn = 0f;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            count++; //10 schuss -> Destroy Stone
            if (!CheckUnderStone(transform.position))  //block unter dem Stein?
            {
                StartCoroutine(Starting());
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!CheckUnderStone(transform.position))
            {
                StartCoroutine(Starting());
            }
        }
    }
    private IEnumerator Starting()
    {
        startAnim = true;
        animator.SetBool("startAnim", startAnim);
        yield return new WaitForSeconds(0.7f);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.gravityScale = 2.5f;
        rb.freezeRotation = true;
    }
    void Update()
    {
        if (rb.linearVelocity != Vector2.zero) //stein f�llt
        {
            isFalling = true;
            startAnim = false;
            animator.SetBool("startAnim", startAnim);
        }
        if (rb.linearVelocity == Vector2.zero && isFalling && transform.position.y <= startPos.y - 1f) //stein is gelandet
        {
            animator.SetBool("startAnim", startAnim);
            isFalling = false;
            started = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.gravityScale = 0f;

        }

        if (!CheckUnderStone(transform.position) && !CheckNeighbourBlocks() && !started && !isFalling)
        {
            started = true;
            StartCoroutine(Starting());
        }
        if (isBurning)
        {
            childObject.SetActive(true);
            if (!isFalling && !CheckUnderStone(transform.position))
            {
                started = true;
                StartCoroutine(Starting());
            }
        }
        else
        {
            childObject.SetActive(false);
        }
        if (count == 10)
        {
            Destroy(gameObject);
        }
    }


    public void BurnStone()
    {
        isBurning = true;
        burn += 1;
        if (burn == 1)
        {
            StartCoroutine(BurnEnd());
        }
    }
    private IEnumerator BurnEnd()
    {
        while (burn != 0)
        {
            yield return new WaitForSeconds(1f);
            count++;
            yield return new WaitForSeconds(1f);
            count++;
            yield return new WaitForSeconds(1f);
            count++;
            yield return new WaitForSeconds(1f);
            count++;
            burn--;
        }
        isBurning = false;
    }


    public bool CheckUnderStone(Vector2 stonePos)
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.6f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.1f);

        // Pr�fen, ob etwas getroffen wurde und ob es nicht im "undestroyable"-Layer ist
        if (!(hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("undestroyable")))
        {
            return false;
        }
        return true;

    }
    public bool CheckNeighbourBlocks()
    {
        Vector2 origin = new Vector2(transform.position.x + 0.8f, transform.position.y);
        Vector2 origin2 = new Vector2(transform.position.x - 0.8f, transform.position.y);
        Vector2 origin3 = new Vector2(transform.position.x, transform.position.y + 0.8f);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, 0.1f);
        RaycastHit2D hit2 = Physics2D.Raycast(origin2, Vector2.left, 0.1f);
        RaycastHit2D hit3 = Physics2D.Raycast(origin3, Vector2.up, 0.1f);

        int count = 0;
        if (hit.collider != null && hit2.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("fallingStone"))
            {
                count++;
            }
            if (hit2.collider.gameObject.CompareTag("fallingStone"))
            {
                count++;
            }
            if (count == 2)
            {
                return true;
            }
        }

        if (hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("undestroyable"))
        {

            if (!hit.collider.gameObject.CompareTag("fallingStone"))
            {
                return true;
            }

        }
        if (hit2.collider != null && hit2.collider.gameObject.layer != LayerMask.NameToLayer("undestroyable"))
        {

            if (!hit2.collider.gameObject.CompareTag("fallingStone"))
            {
                return true;
            }
        }
        if (hit3.collider != null)
        {
            if (hit3.collider.gameObject.layer == LayerMask.NameToLayer("undestroyable"))
            {
                return false;
            }
            if (hit3.collider.gameObject.CompareTag("fallingStone"))
            {
                return false;
            }
            return true;
        }
        return false;
    }
}
