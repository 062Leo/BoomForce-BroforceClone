using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class oilBarrelSkript : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private float explTime;
    [SerializeField] private LayerMask undestroyableLayer;

    private Rigidbody2D rb;
    public AudioSource source;
    public AudioClip clip;
    private GameObject childObject;

    private bool isTriggered;
    private bool isExploded;
    private bool isFalling;

    private void Awake()
    {
        isTriggered = false;
        isExploded = false;
        childObject = transform.GetChild(0).gameObject;
        map = GameObject.Find("Destroyable").GetComponent<Tilemap>();
        tileManager = GameObject.Find("Destroyable").GetComponent<TileManager>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            isTriggered = true;
            Debug.Log("a");
            childObject.SetActive(true);
            Vector3Int tilePosition = map.WorldToCell(transform.position);
            StartCoroutine(tileManager.TileExplosionRoutine(explTime, tilePosition));
            Invoke("DestroyBarrel", explTime + 0.07f);
        }
    }

    public void ExplodeOilBarrel()
    {
        if (!isExploded)
        {
            isExploded = true;
            childObject.SetActive(true);
            Vector3Int tilePosition = map.WorldToCell(transform.position);
            StartCoroutine(tileManager.TileExplosionRoutine(0.2f, tilePosition));
            Invoke("DestroyBarrel", 0.3f);
        }
    }
    public void TriggerOilBarrel()
    {
        if (!isTriggered&&!isExploded)
        {
           
            isTriggered =true;
            childObject.SetActive(true);
            Vector3Int tilePosition = map.WorldToCell(transform.position);
            StartCoroutine(tileManager.TileExplosionRoutine(explTime, tilePosition));
            Invoke("DestroyBarrel", explTime + 0.05f);
        }
    }

    public bool CheckUnderOilBarrel()
    {
        // Position direkt unter dem GameObject bestimmen
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.6f);
        Vector2 direction = Vector2.down; // Richtung nach unten

        // Raycast nach unten abfeuern
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 0.1f);

        // Prüfen, ob etwas getroffen wurde und ob es nicht im "undestroyable"-Layer ist
        if (hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("undestroyable"))
        {
            return true; // Es wurde ein Objekt gefunden
        }

        return false; // Kein Objekt gefunden
    }
    void FixedUpdate()
    {
        if (!CheckUnderOilBarrel())
        {

            StartCoroutine(Falling());
        }
        else
        {
            if (isFalling)
            {
                isFalling = false;
                childObject.SetActive(true);
                Vector3Int tilePosition = map.WorldToCell(transform.position);
                StartCoroutine(tileManager.TileExplosionRoutine(0, tilePosition));
                Invoke("DestroyBarrel", 0.05f);
            }
        }

    }
    private IEnumerator Falling()
    {
        isFalling = true;
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
}
