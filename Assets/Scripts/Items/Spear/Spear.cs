using System.Collections;
using UnityEngine;

public class Spear : Item
{
    public float stabDistance = 2f;
    public float stabSpeed = 10f;
    public int damage = 20;
    
    private Vector3 originalPosition;
    private bool isStabbing;

    public override void Use()
    {
        if(!isStabbing)
            StartCoroutine(StabAttack());
    }

    IEnumerator StabAttack()
    {
        isStabbing = true;
        originalPosition = transform.localPosition;
        
        // Stab forward
        while(Vector3.Distance(transform.localPosition, originalPosition) < stabDistance)
        {
            transform.Translate(Vector3.forward * stabSpeed * Time.deltaTime);
            yield return null;
        }

        // Check for hits
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, stabDistance))
        {
            // if(hit.collider.TryGetComponent<Health>(out Health health))
            // {
            //     health.TakeDamage(damage);
            // }
        }

        // Return to original position
        while(transform.localPosition != originalPosition)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, stabSpeed * Time.deltaTime);
            yield return null;
        }
        
        isStabbing = false;
    }
}