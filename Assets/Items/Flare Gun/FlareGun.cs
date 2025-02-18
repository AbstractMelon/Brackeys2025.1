using UnityEngine;

public class FlareGun : Item
{
    // Fields to store flare gun properties
    public Transform muzzleTransform;
    public GameObject flarePrefab;
    public float fireRate = 1f;
    private float nextFireTime = 0f;

    // Override the Use method to fire a flare
    public override void Use()
    {
        if (Time.time >= nextFireTime)
        {
            FireFlare();
            nextFireTime = Time.time + fireRate;
        }
    }

    // Method to handle firing the flare
    private void FireFlare()
    {
        if (flarePrefab != null && muzzleTransform != null)
        {
            Instantiate(flarePrefab, muzzleTransform.position, muzzleTransform.rotation);
            Debug.Log("Flare fired!");
        }
    }

    // Override the DefaultRotation method
    public override Quaternion DefaultRotation()
    {
        return Quaternion.Euler(-90f, 0f, 0f);
    }
}

