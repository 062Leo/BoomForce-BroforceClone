using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.05f;
    public AudioSource source;
    public AudioClip clip;

    private float nextFireTime;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = transform.parent.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Don't shoot if the game is paused
        if (playerMovement != null && playerMovement.isPaused)
            return;
            
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; 
        }
    }
    void Shoot()
    {
        source.PlayOneShot(clip);
        Instantiate(bulletPrefab,firePoint.position,firePoint.rotation);
    }
}
