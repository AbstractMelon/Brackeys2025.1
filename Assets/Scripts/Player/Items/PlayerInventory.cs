using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public InventoryItem[] items;
    public int heldSlot = 0;
    public int totalSlots;
    public GameObject inventoryItemPrefab;
    public float selectedItemScale;
    void Awake()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(600, 180);
        items = new InventoryItem[totalSlots];
        for (int i = 0; i < totalSlots; i++)
        {
            items[i] = Instantiate(inventoryItemPrefab, transform.GetChild(i)).GetComponent<InventoryItem>();
            items[i].inventory = this;
        }
        ActivateSlot(heldSlot);
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
        if (items[heldSlot].itemObject == null) 
        {
            items[heldSlot].itemObject = item;
            item.item = items[heldSlot];
            return true;
        }
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemObject == null)
            {
                items[i].itemObject = item;
                item.item = items[i];
                items[i].StopHolding();
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
        DeactivateSlot(heldSlot);
        items[heldSlot].StopHolding();
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
        ActivateSlot(heldSlot);
        items[heldSlot].Hold();
    }
    public bool SetHeldSlot(int slot)
    {
        if (slot < 0 || slot > totalSlots) return false;
        DeactivateSlot(heldSlot);
        items[heldSlot].StopHolding();
        heldSlot = slot;
        ActivateSlot(heldSlot);
        items[heldSlot].Hold();
        return true;
    }
    public void ActivateSlot(int slot)
    {
        transform.GetChild(slot).GetChild(0).GetComponent<Image>().color = Color.gray;
    }
    public void DeactivateSlot(int slot)
    {
        transform.GetChild(slot).GetChild(0).GetComponent<Image>().color = Color.white;
    }
}
