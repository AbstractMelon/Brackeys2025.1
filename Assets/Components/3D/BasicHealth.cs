using UnityEngine;
using UnityEngine.Events;

public class BasicHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100;
    public UnityEvent OnDeath;

    [Header("Debug View")]
    [SerializeField] private float currentHealth;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0) Die();
    }

    void Die()
    {
        OnDeath.Invoke();
        Destroy(gameObject);
    }
}