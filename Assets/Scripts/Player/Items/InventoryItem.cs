using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public PlayerInventory inventory;
    public Item itemObject;
    public void Use()
    {
        itemObject.Use();
    }
}
