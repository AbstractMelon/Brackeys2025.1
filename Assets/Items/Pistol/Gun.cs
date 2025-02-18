using UnityEngine;
public class Gun : Item
{
    public Transform muzzleTransform;
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public float fireRate = 0.5f;
    private float nextFireTime;
    public override Quaternion DefaultRotation()
    {
        return Quaternion.Euler(-90f, 90f, 0f);
    }

    public override void Use()
    {
        if(Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if(bulletPrefab && muzzleTransform)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzleTransform.position, muzzleTransform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if(rb) rb.AddForce(muzzleTransform.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}