using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.05f;
    public AudioSource source;
    public AudioClip clip;

    private float nextFireTime;

    void Update()
    {
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
