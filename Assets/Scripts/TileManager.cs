using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    private Tilemap tilemap;
    private MapManager mapManager;
    private FlyBarrel flybarrel;
    public GameObject burningPrefab;
    public GameObject explosionPrefab;
    public AudioSource source;
    public AudioClip clip;
    private HashSet<GameObject> detectedExplodeObjects = new HashSet<GameObject>();
    private HashSet<GameObject> detectedTriggerObjects = new HashSet<GameObject>();

    private List<Vector3Int> burningList = new List<Vector3Int>();
    private List<Vector3Int> explosionList = new List<Vector3Int>();
    private static Dictionary<Vector3Int, int> tileHealth;
    private void Awake()
    {
        mapManager = FindObjectOfType<MapManager>();
    }

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tileHealth = new Dictionary<Vector3Int, int>();
    }
    public void DestroyTile(Vector3Int tilepos)
    {
        Vector2 pos = new Vector2(tilepos.x, tilepos.y);
        if (mapManager.getTileName(pos) == "ground")
        {
            tilemap.SetTile(tilepos, null); // Tile zerstören
            tileHealth.Remove(tilepos);
        }
        if (mapManager.getTileName(pos) == "explosive")
        {
            float tileExplTime = mapManager.getTileExplosionTimer(pos);
            StartCoroutine(TileExplosionRoutine(tileExplTime, tilepos));
        }
    }
    public void DamageTile(Vector2 position)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(position);
        float health;
        try
        {
            health = mapManager.GetTileHealth(position);
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"KeyNotFoundException: The given key '{position}' was not present in the dictionary. {e.Message}");
            return;
        }

        if (mapManager.getTileDestroyable(position))
        {

            if (tileHealth.ContainsKey(tilePosition))
            {
                tileHealth[tilePosition]++;
            }
            else
            {
                tileHealth[tilePosition] = 1;
            }
            if (tileHealth[tilePosition] >= health)
            {
                DestroyTile(tilePosition);
            }
        }
        if (mapManager.getTileName(position) == "explosive")
        {
            Vector3 spawnPosition = new Vector3(tilePosition.x + 0.5f, tilePosition.y + 0.5f, 0f); // Adjust the z-coordinate as needed
            Instantiate(burningPrefab, spawnPosition, Quaternion.identity);

            float tileExplTime = mapManager.getTileExplosionTimer(position);
            StartCoroutine(TileExplosionRoutine(tileExplTime, tilePosition));
        }
    }
    public IEnumerator TileExplosionRoutine(float delay, Vector3Int tilePosition)
    {
        yield return new WaitForSeconds(delay);
        if (!explosionList.Contains(tilePosition))
        {
            Vector3 spawnPosition = new Vector3(tilePosition.x + 0.5f, tilePosition.y + 0.5f, 0f); // Adjust the z-coordinate as needed
            Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
            explosionList.Add(tilePosition);
            StartCoroutine(RemoveFromExplList(tilePosition));
            source.PlayOneShot(clip);
        }
        tilemap.SetTile(tilePosition, null); // Tile zerstören
        tileHealth.Remove(tilePosition);

        // Behandle die inneren 9 Kacheln
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighborPosition = new Vector3Int(tilePosition.x + x, tilePosition.y + y, tilePosition.z);
                TileBase neighborTile = tilemap.GetTile(neighborPosition);
                if (neighborTile != null)
                {
                    DestroyTile(neighborPosition);
                }
            }
        }
        // Behandle die äußeren 16 Kacheln
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (Mathf.Abs(x) == 2 || Mathf.Abs(y) == 2) // Nur äußere Kacheln
                {
                    Vector3Int outerNeighborPosition = new Vector3Int(tilePosition.x + x, tilePosition.y + y, tilePosition.z);
                    TileBase neighborTile = tilemap.GetTile(outerNeighborPosition);
                    if (neighborTile != null)
                    {
                        if (!burningList.Contains(outerNeighborPosition))
                        {
                            StartCoroutine(BurnAnim(outerNeighborPosition));
                            burningList.Add(outerNeighborPosition);
                            StartCoroutine(RemoveFromBurnList(outerNeighborPosition));
                        }
                        StartCoroutine(BurnTile(1, outerNeighborPosition));
                        StartCoroutine(BurnTile(2, outerNeighborPosition));
                        StartCoroutine(BurnTile(3, outerNeighborPosition));
                    }
                }
            }
        }
        Vector2 explosionPosition = tilemap.CellToWorld(tilePosition);
        explosionPosition.x += 0.5f;
        explosionPosition.y += 0.5f;

        RaycastHit2D[] hitsSmall = Physics2D.CircleCastAll(explosionPosition, 1.4f, Vector2.zero, 0f); //0.8
        foreach (RaycastHit2D hit in hitsSmall)
        {
            if (hit.collider.CompareTag("fallingStone"))
            {
                Destroy(hit.collider.gameObject);
            }
            if (hit.collider.gameObject.CompareTag("oilBarrel"))
            {
                if (hit.collider.gameObject.transform.position == tilePosition)
                {
                    detectedExplodeObjects.Add(hit.collider.gameObject);
                }
                float distance = Vector2.Distance(explosionPosition, hit.collider.gameObject.transform.position);
                if (distance > 0.8f)
                {

                    if (!detectedExplodeObjects.Contains(hit.collider.gameObject))
                    {
                        detectedExplodeObjects.Add(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<oilBarrelSkript>().ExplodeOilBarrel();
                    }

                }
            }
        }

        RaycastHit2D[] hitsBig = Physics2D.CircleCastAll(explosionPosition, 2.4f, Vector2.zero, 0f);
        foreach (RaycastHit2D hit in hitsBig)
        {
            if (hit.collider.gameObject.CompareTag("oilBarrel"))
            {
                if (hit.collider.gameObject.transform.position == tilePosition)
                {
                    detectedTriggerObjects.Add(hit.collider.gameObject);
                }
                float distance = Vector2.Distance(explosionPosition, hit.collider.gameObject.transform.position);
                if (distance > 1.8f)
                {
                    if (!detectedTriggerObjects.Contains(hit.collider.gameObject))
                    {
                        detectedTriggerObjects.Add(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<oilBarrelSkript>().TriggerOilBarrel();
                    }
                }
            }
            if (hit.collider.CompareTag("fallingStone"))
            {
                hit.collider.gameObject.GetComponent<FallingStone>().BurnStone();
            }

        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPosition, 2.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("flyBarrel"))
            {
                flybarrel = collider.gameObject.GetComponent<FlyBarrel>();
                flybarrel.TriggerFlyBarrel();
            }
        }
    }

    private void OnDrawGizmos() //visualisieren des explosionsradius eines barrels ( nur in editor ansicht zum debugen)
    {
        // Festgelegte Werte
        Vector2 explosionPosition = new Vector2(-61.5f, 17.5f); // Position des Kreises
        float radius = 1.4f; // Radius des Kreises

        // Zeichne den Kreis an der angegebenen Position mit dem angegebenen Radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(explosionPosition, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(explosionPosition, 2.4f);

        // Zeichne einen kleinen Punkt am Kreismittelpunkt
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(explosionPosition, 0.05f);

        // Führe den CircleCast durch und speichere die Treffer
        RaycastHit2D[] hits = Physics2D.CircleCastAll(explosionPosition, radius, Vector2.zero, 0f);

        // Zeichne Treffer als grüne Linien
        Gizmos.color = Color.green;
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                Gizmos.DrawLine(explosionPosition, hit.point);
                Gizmos.DrawSphere(hit.point, 0.1f); // Markiere die Trefferpunkte
            }
        }
    }
    private IEnumerator RemoveFromBurnList(Vector3Int pos)
    {
        yield return new WaitForSeconds(3f);
        burningList.Remove(pos);
    }
    private IEnumerator RemoveFromExplList(Vector3Int pos)
    {
        yield return new WaitForSeconds(30f);
        burningList.Remove(pos);
    }
    public IEnumerator BurnAnim(Vector3Int tilepos)
    {
        yield return new WaitForSeconds(0.2f);
        Vector3 spawnPosition = new Vector3(tilepos.x + 0.5f, tilepos.y + 0.5f, 0f); // Adjust the z-coordinate as needed
        Instantiate(burningPrefab, spawnPosition, Quaternion.identity);
    }
    private IEnumerator BurnTile(float delay, Vector3Int tilepos)
    {
        yield return new WaitForSeconds(delay);
        Vector2 pos = new Vector2(tilepos.x, tilepos.y);
        if (mapManager.getTileName(pos) == "ground")
        {
            DamageTile(pos);
            DamageTile(pos);
        }
        if (mapManager.getTileName(pos) == "explosive")
        {
            float tileExplTime = mapManager.getTileExplosionTimer(pos);
            StartCoroutine(TileExplosionRoutine(tileExplTime, tilepos));
        }
    }
}
