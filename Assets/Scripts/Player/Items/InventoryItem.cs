using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    // This class shouldn't need modified much and should work as is.
    public PlayerInventory inventory;
    public Item itemObject;
    public void Use()
    {
        itemObject.Use();
    }
    public void Drop()
    {
        itemObject.Drop();
    }
    public void Hold()
    {
        itemObject.Hold();
    }
    public void StopHolding()
    {
        itemObject.StopHolding();
    }
}
