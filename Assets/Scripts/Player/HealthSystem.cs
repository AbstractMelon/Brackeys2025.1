using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

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
        gameObject.transform.SetParent(GameObject.Find("SpectatorPosition").transform);
        transform.localPosition = Vector3.zero;
        transform.GetChild(0).gameObject.SetActive(false);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        GameManager.instance.CheckGameOver();
    }
}

