using System.Collections;
using UnityEngine;

public class Axe : Item
{
    public float swingDistance = 1.5f;
    public float swingSpeed = 10f;
    public int damage = 20;

    private Vector3 originalPosition;
    private bool isSwinging;

    public override void Use()
    {
        if (!isSwinging)
            StartCoroutine(SwingAttack());
    }

    IEnumerator SwingAttack()
    {
        isSwinging = true;
        originalPosition = transform.localPosition;

        // Swing forward
        while (Vector3.Distance(transform.localPosition, originalPosition) < swingDistance)
        {
            transform.Translate(Vector3.forward * swingSpeed * Time.deltaTime);
            yield return null;
        }

        // Check for hits
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, swingDistance))
        {
            if (hit.collider.TryGetComponent<HealthSystem>(out HealthSystem health))
            {
                health.TakeDamage(damage);
            }
        }

        // Return to original position
        while (transform.localPosition != originalPosition)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, swingSpeed * Time.deltaTime);
            yield return null;
        }

        isSwinging = false;
    }
}
