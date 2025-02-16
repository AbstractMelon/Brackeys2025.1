using UnityEditor.Search;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryItem[] items = new InventoryItem[5];
    public int heldSlot;

    public bool HasOpenSpace()
    {
        return GetItemCount() < 5;
    }
    public int GetItemCount()
    {
        int count = 0;
        foreach (InventoryItem item in items)
        {
            if (item != null) count++;
        }
        return count;
    }
    public bool PickupItem(InventoryItem item)
    {
        if (!HasOpenSpace()) return false;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                item.inventory = this;
                item.transform.parent = transform;
                item.transform.position = ItemSlotToPosition(i);
                return true;
            }
        }
        return false;
    }
    public Vector2 ItemSlotToPosition(int i)
    {
        return Vector2.zero;
    }
    public bool DiscardHeldItem()
    {
        if (items[heldSlot] == null) return false;
        items[heldSlot].Use();
        items[heldSlot] = null;
        return true;
    }
}
