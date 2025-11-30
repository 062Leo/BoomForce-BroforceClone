using System.Collections;
using UnityEngine;

public class DeleteExplosionPref : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DeletePrefDelay()); 
    }
    private IEnumerator DeletePrefDelay()
    {
        yield return new WaitForSeconds(0.53f);
        Destroy(gameObject);
    }
    
}
