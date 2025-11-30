using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class FlyBarrel : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private float maxSpeed = 10f;  // Maximale Geschwindigkeit
    [SerializeField] private float accelerationTime = 1f;  // Zeit in Sekunden, bis die maximale Geschwindigkeit erreicht ist

    private Rigidbody2D rb;
    private PlayerMovement player;
    private GameObject childObject;

    private Vector3 explPos;
    private Vector3 startPos;

    private float timeElapsed = 0f;
    private int dir; //0= Up ; 1=right //direction von Flybarrel

    private bool isFlying;
    private bool flying;
    private bool started;
    private bool isFalling;
    private bool isExploding;

    private void Awake()
    {
        isExploding = false;
        childObject = transform.GetChild(0).gameObject;
        started = false;
        isFlying = false;
        startPos = transform.position;
        flying = false;
        map = GameObject.Find("Destroyable").GetComponent<Tilemap>();
        tileManager = GameObject.Find("Destroyable").GetComponent<TileManager>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (isFlying)
        {
            timeElapsed += Time.deltaTime;

            // Berechne den aktuellen Geschwindigkeitsfaktor basierend auf der verstrichenen Zeit
            float speedFactor = Mathf.Clamp01(timeElapsed / accelerationTime);
            float currentSpeed = speedFactor * maxSpeed;

            // Setze die vertikale Geschwindigkeit des Rigidbody2D
            if (dir == 0)
            {
                rb.linearVelocity = new Vector2(0, currentSpeed);
            }
            if (dir == 1)
            {
                rb.linearVelocity = new Vector2(currentSpeed, 0);
            }
            // Setze die Geschwindigkeit auf maxSpeed, nachdem die Beschleunigungszeit vorbei ist
            if (timeElapsed >= accelerationTime)
            {
                if (dir == 0)
                {
                    rb.linearVelocity = new Vector2(0, maxSpeed);
                }
                if (dir == 1)
                {
                    rb.linearVelocity = new Vector2(maxSpeed, 0);
                }
            }
            if (dir == 0)  
            {
                if (transform.position.y >= explPos.y && !isExploding)
                {
                    isExploding = true;
                    Vector2 position = transform.position;
                    Vector3Int tilePosition = map.WorldToCell(position);
                    StartCoroutine(tileManager.TileExplosionRoutine(0, tilePosition));
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                    Invoke("DestroyBarrel", 0.1f);
                }
            }
            if (dir == 1)
            {
                if (transform.position.x >= explPos.x && !isExploding)
                {
                    isExploding = true;
                    Vector2 position = transform.position;
                    Vector3Int tilePosition = map.WorldToCell(position);
                    StartCoroutine(tileManager.TileExplosionRoutine(0f, tilePosition));
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                    Invoke("DestroyBarrel", 0.1f);
                }
            }
        }
        else
        {
            if (!CheckUnderFlyBarrel(transform.position) && !started)
            {
                StartCoroutine(Falling());
            }
            if (rb.linearVelocity == Vector2.zero && isFalling && !flying)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                rb.gravityScale = 0f;
                float multiplied = transform.position.y * 2;
                float rounded = ((float)Math.Ceiling(multiplied)) / 2;
                transform.position = new Vector3(transform.position.x, rounded + 0.05f, transform.position.z);
                isFalling = false;

                SetUpFlyingDirection();
                StartCoroutine(Starting());
                explPos = startPos;
                explPos.y = explPos.y + 18;
                explPos.x = explPos.x + 18;

            }
            if (rb.linearVelocity != Vector2.zero && !flying)
            {
                isFalling = true;
            }
        }
    }


    public void startFlyBarrel()
    {
        isFlying = true;
        timeElapsed = 0f;
    }
    private IEnumerator Starting()
    {
        started = true;
        yield return new WaitForSeconds(1.5f);
        if (!flying && !isFalling)
        {
            childObject.SetActive(true);
            flying = true;
            rb.constraints = RigidbodyConstraints2D.None;
            if (dir == 0)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            }
            else
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            }
            rb.freezeRotation = true;
            isFlying = true;
            timeElapsed = 0f;
            source.PlayOneShot(clip);
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bullet") && !flying)
        {
            SetUpFlyingDirection();
            StartCoroutine(Starting());
            explPos = startPos;
            explPos.y = explPos.y + 18;
            explPos.x = explPos.x + 18;
        }
        Debug.Log("coll");
        if (isFlying)
        {
            Debug.Log("isFlying");
            if (!collision.gameObject.CompareTag("unDestroyable") && !collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("destroy Collision");
                MapManager mapManager;
                mapManager = FindObjectOfType<MapManager>();
                Vector2 position = transform.position;
                Vector3Int tilePosition = map.WorldToCell(position);
                if (dir == 1)
                {
                    position.x = position.x + 2;
                    tilePosition.x = tilePosition.x + 2;
                    Debug.Log("dir hoch");
                }
                if (dir == 0)
                {
                    position.y = position.y + 2;
                    tilePosition.y = tilePosition.y + 2;
                    Debug.Log("dir rechts");
                }
                if (mapManager.getTileName(position) == "explosive")
                {
                    StartCoroutine(tileManager.TileExplosionRoutine(0f, tilePosition));
                    Debug.Log("explosive coll");
                }
                else
                {
                    Debug.Log("DestroyTile " + tilePosition);
                    tileManager.DestroyTile(tilePosition);
                }
            }
            if (collision.gameObject.CompareTag("flyBarrel"))
            {
                Vector2 position = transform.position;
                Vector3Int tilePosition = map.WorldToCell(position);
                StartCoroutine(tileManager.TileExplosionRoutine(0f, tilePosition));
                rb.constraints = RigidbodyConstraints2D.FreezePositionY;
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                Invoke("DestroyBarrel", 0.05f);
            }
            if (collision.gameObject.CompareTag("oilBarrel"))
            {
                collision.gameObject.GetComponent<oilBarrelSkript>().ExplodeOilBarrel();
            }
        }
    }

    private void SetUpFlyingDirection()
    {
        if (!flying)
        {
            float zRotation = transform.eulerAngles.z;
            if (Mathf.Approximately(zRotation, 0f))
            {
                dir = 0;
            }
            if (Mathf.Approximately(zRotation, 270f))
            {
                dir = 1;
            }
        }
    }

    public bool CheckUnderFlyBarrel(Vector2 flyBarrelPos)
    {
        Vector3Int tilePosition = map.WorldToCell(flyBarrelPos);
        Vector3Int neighborPosition = new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z);
        TileBase neighborTile = map.GetTile(neighborPosition);
        if (neighborTile == null)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(map.CellToWorld(neighborPosition), Vector2.one, 0);

            foreach (Collider2D collider in colliders)
            {
                // �berpr�fe, ob das Collider-Objekt das gew�nschte Tag hat
                if (collider.gameObject.CompareTag("flyBarrel") || collider.gameObject.CompareTag("fallingStone"))
                {
                    return true;
                }
            }
            return false;
        }
        return true;
    }

    private IEnumerator Falling()
    {
        yield return new WaitForSeconds(0.7f);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 2.5f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb.freezeRotation = true;
    }

    public void DestroyBarrel()
    {
        Destroy(gameObject);
    }

    public void TriggerFlyBarrel()
    {
        if (!flying)
        {
            SetUpFlyingDirection();
            StartCoroutine(Starting());
            explPos = transform.position;
            explPos.y = explPos.y + 18;
            explPos.x = explPos.x + 18;
        }
    }
}
