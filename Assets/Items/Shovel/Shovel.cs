using UnityEngine;

public class Shovel : ThrowableItem
{
    public float range = 5f;
    public float radius = 2f;

    public override void Use()
    {
        base.Use();

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Dirt"))
            {
                Destroy(collider.gameObject);
            }
        }
    }
}
