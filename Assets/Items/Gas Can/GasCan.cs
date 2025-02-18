using System.Collections;
using UnityEngine;

public class GasCan : MonoBehaviour
{
    public float fuelAmount = 100f;
    public float maxFuelCapacity = 100f;

    public void Use(GameObject player)
    {
        Destroy(gameObject);
    }

    // Method to refill the gas can
    public void Refill(float amount)
    {
        fuelAmount = Mathf.Min(fuelAmount + amount, maxFuelCapacity);
    }
}

