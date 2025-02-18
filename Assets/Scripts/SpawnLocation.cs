using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public ParticleSystem spawnEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!spawnEffect)
        {
            spawnEffect = GetComponent<ParticleSystem>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject)
            {
                PlaySpawnEffect();
                break;
            }
        }
        
    }

    void PlaySpawnEffect()
    {
        spawnEffect.transform.position = transform.position;
        spawnEffect.Play();
    }
}
