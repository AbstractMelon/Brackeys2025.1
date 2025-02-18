using System.Collections;
using UnityEngine;

public class BearTrap : Item
{
    public float triggerDistance = 0.5f;
    public float triggerSpeed = 10f;
    public int damage = 20;

    private Vector3 originalPosition;
    private bool isTriggered;

    public override void Use()
    {
        if (!isTriggered)
            StartCoroutine(TriggerAttack());
    }

    IEnumerator TriggerAttack()
    {
        isTriggered = true;
        originalPosition = transform.localPosition;

        // Trigger forward
        while (Vector3.Distance(transform.localPosition, originalPosition) < triggerDistance)
        {
            transform.Translate(Vector3.forward * triggerSpeed * Time.deltaTime);
            yield return null;
        }

        // Check for hits
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, triggerDistance))
        {
            if (hit.collider.TryGetComponent<HealthSystem>(out HealthSystem health))
            {
                health.TakeDamage(damage);
            }
        }

        // Return to original position
        while (transform.localPosition != originalPosition)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, triggerSpeed * Time.deltaTime);
            yield return null;
        }

        isTriggered = false;
    }
}
