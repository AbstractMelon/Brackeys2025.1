using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public InventoryItem[] items;
    public int heldSlot;
    public int totalSlots;
    public GameObject inventoryItemPrefab;
    void Awake()
    {
        items = new InventoryItem[totalSlots];
        for (int i = 0; i < totalSlots; i++)
        {
            items[i] = Instantiate(inventoryItemPrefab, transform.GetChild(i)).GetComponent<InventoryItem>();
            items[i].inventory = this;
        }
    }

    public bool HasOpenSpace()
    {
        return GetItemCount() < 5;
    }
    public int GetItemCount()
    {
        int count = 0;
        foreach (InventoryItem item in items)
        {
            if (item.itemObject != null) count++;
        }
        return count;
    }
    public bool PickupItem(Item item)
    {
        if (!HasOpenSpace()) return false;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemObject == null)
            {
                items[i].itemObject = item;
                item.item = items[i];
                return true;
            }
        }
        return false;
    }
    public Vector2 ItemSlotToScreenPosition(int slot)
    {
        return Vector2.zero;
    }
    public bool DiscardHeldItem(bool drop = true)
    {
        if (items[heldSlot].itemObject == null) return false;
        if (!drop) Destroy(items[heldSlot].gameObject);
        else items[heldSlot].Drop();
        items[heldSlot].itemObject = null;
        items[heldSlot].GetComponent<Image>().sprite = null;
        
        return true;
    }
    public bool UseHeldItem()
    {
        if (items[heldSlot].itemObject == null) return false;
        items[heldSlot].Use();
        return true;
    }
    public void Scroll(bool left)
    {
        if (left)
        {
            heldSlot--;
            if (heldSlot < 0)
            {
                heldSlot += totalSlots;
            }
        }
        else
        {
            heldSlot++;
            if (heldSlot >= totalSlots)
            {
                heldSlot -= totalSlots;
            }
        }
    }
}
