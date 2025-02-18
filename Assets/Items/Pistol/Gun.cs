using UnityEngine;

public class Gun : Item
{
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime;

    public override Quaternion DefaultRotation()
    {
        return Quaternion.Euler(-90f, 90f, 0f);
    }

    public override void Use()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            if (impactEffectPrefab)
            {
                GameObject impactEffect = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactEffect, 2f);
            }

            HealthSystem targetHealth = hit.transform.GetComponent<HealthSystem>();
            if (targetHealth)
            {
                targetHealth.TakeDamage(10); 
            }
        }
    }
}

