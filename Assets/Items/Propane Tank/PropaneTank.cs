using System.Collections;
using UnityEngine;

namespace Items
{
    public class PropaneTank : Item
    {
        public float fuelAmount = 100f;
        public float maxFuelAmount = 100f;
        public float fuelUsage = 0.1f;
        public GameObject explosionEffect;
        public float explosionRadius = 5f;
        public float explosionForce = 700f;

        public override void Use()
        {
            base.Use();

            if (fuelAmount > 0f)
            {
                fuelAmount -= fuelUsage * Time.deltaTime;
            }
            else
            {
                Explode();
                item.inventory.DiscardHeldItem();
            }
        }

        private void Explode()
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }

            Destroy(gameObject);
        }
    }
}

