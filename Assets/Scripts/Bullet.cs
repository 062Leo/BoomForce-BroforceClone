using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    private Vector3 startPosition;
    private static HashSet<Vector3Int> explosionStartedPositions = new HashSet<Vector3Int>();
    private MapManager mapManager;
    void Start()
    {
        startPosition = transform.position;
        rb.linearVelocity = transform.right * speed;
    }
    void Update()
    {
        float distanceTravelled = transform.position.x - startPosition.x;
        if (Mathf.Abs(distanceTravelled) >= 10f)
        {
            Destroy(gameObject);
        }
    }
    private void Awake()
    {
        mapManager = FindObjectOfType<MapManager>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("destroyableTileMap"))
        {
            Vector3 hitPosition = Vector3.zero;
            foreach (ContactPoint2D hit in collision.contacts)
            {
                hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
                if (tilemap != null)
                {
                    Vector2 tilepos2d = new Vector2(hitPosition.x, hitPosition.y);
                    Vector3Int tilePosition = tilemap.WorldToCell(hitPosition);
                    if (mapManager.getTileName(tilepos2d) == "explosive" || mapManager.getTileName(tilepos2d) == "flyingExplosive")
                    {
                        if (!explosionStartedPositions.Contains(tilePosition))
                        {
                            explosionStartedPositions.Add(tilePosition);
                            tilemap.GetComponent<TileManager>().DamageTile(tilepos2d);
                        }
                    }


                    if (mapManager.getTileName(tilepos2d) == "ground")
                    {
                        tilemap.GetComponent<TileManager>().DamageTile(tilepos2d);
                    }
                }
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("flyBarrel")|| collision.gameObject.CompareTag("fallingStone") || collision.gameObject.CompareTag("oilBarrel"))
        {
            Destroy(gameObject);
        }
    }
}
