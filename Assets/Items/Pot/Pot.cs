using UnityEngine;

public class Pot : MonoBehaviour
{
    public float durability;
    public bool isFilled;

    void Start()
    {
        durability = 100f;
        isFilled = false;
    }

    public void Fill()
    {
        if (!isFilled)
        {
            isFilled = true;
            Debug.Log("Pot is now filled.");
        }
    }

    public void Empty()
    {
        if (isFilled)
        {
            isFilled = false;
            Debug.Log("Pot is now empty.");
        }
    }

    public void Use()
    {
        if (isFilled)
        {
            // Use the pot's contents
            durability -= 10f;
            Empty();
            Debug.Log("Used the pot. Remaining durability: " + durability);
        }
        else
        {
            Debug.Log("Pot is empty and cannot be used.");
        }
    }
}

