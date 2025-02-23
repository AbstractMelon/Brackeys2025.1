using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public AudioClip hurtSFX;
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // press 'H' to take damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Health: " + currentHealth);

        if (hurtSFX != null)
        {
            audioSource.PlayOneShot(hurtSFX);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Health: " + currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died");
        gameObject.tag = "spectatorschat";
        gameObject.transform.position = GameObject.Find("SpectatorPosition").transform.position;
        gameObject.transform.rotation = GameObject.Find("SpectatorPosition").transform.rotation;
        gameObject.transform.SetParent(GameObject.Find("SpectatorPosition").transform);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        GameManager.instance.CheckGameOver();
    }
}


