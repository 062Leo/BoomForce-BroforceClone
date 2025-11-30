using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class deleteBurnPrefab : MonoBehaviour
{
    private MapManager mapManager;
    private void Awake()
    {
        mapManager = FindObjectOfType<MapManager>();
    }

    void Update()
    {
        if (mapManager.isThereATile(transform.position))
        {
            if (mapManager.getTileName(transform.position) == "ground")
            {
                StartCoroutine(DeletePrefDelay(3f));
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator DeletePrefDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
