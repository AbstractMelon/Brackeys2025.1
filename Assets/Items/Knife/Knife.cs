using System.Collections;
using UnityEngine;

public class Knife : ThrowableItem
{
    public float stabDamage = 10f;
    public float stabRange = 1f;
    public float stabCooldown = 1f;

    private bool isOnCooldown { get; set; }

    public override void Use()
    {
        if (isOnCooldown) return;

        StartCoroutine(Stab());

        isOnCooldown = true;
        Invoke(nameof(ResetCooldown), stabCooldown);
    }

    private IEnumerator Stab()
    {
        var originalPosition = transform.localPosition;
        var targetPosition = originalPosition + transform.forward * stabRange;

        float timeElapsed = 0;
        while (timeElapsed < stabCooldown)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, timeElapsed / stabCooldown);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (Physics.Raycast(transform.position, transform.forward, out var hit, stabRange))
        {
            var enemyHealth = hit.transform.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(stabDamage);
            }
        }

        transform.localPosition = originalPosition;

        base.Use();
    }

    private void ResetCooldown()
    {
        isOnCooldown = false;
    }
}

