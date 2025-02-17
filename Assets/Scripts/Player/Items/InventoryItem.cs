using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    // This class shouldn't need modified much and should work as is.
    public PlayerInventory inventory;
    public Item itemObject;
    public void Use()
    {
        if (itemObject != null) itemObject.Use();
    }
    public void Drop()
    {
        if (itemObject != null) itemObject.Drop();
    }
    public void Hold()
    {
        if (itemObject != null) itemObject.Hold();
    }
    public void StopHolding()
    {
        if (itemObject != null) itemObject.StopHolding();
    }
}
