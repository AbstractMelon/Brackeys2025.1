using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    private InventoryItem[] items;
    public int heldSlot { get; private set; } = 0;
    public int totalSlots { get; private set; } = 5;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Sprite itemSelectedSprite;
    [SerializeField] private Sprite emptySprite;
    void Awake()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
        items = new InventoryItem[totalSlots];
        for (int i = 0; i < totalSlots; i++)
        {
            RectTransform trans = transform.GetChild(i).GetChild(0).GetComponent<RectTransform>();
            items[i] = Instantiate(inventoryItemPrefab, trans.transform).GetComponent<InventoryItem>();
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
    public bool DiscardHeldItem(bool drop = true)
    {
        if (items[heldSlot].itemObject == null) return false;
        if (!drop) Destroy(items[heldSlot].gameObject);
        else items[heldSlot].Drop();
        items[heldSlot].itemObject = null;
        items[heldSlot].GetComponent<Image>().sprite = emptySprite;
        
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
    public void DestroyItem(Item item)
    {
        for (int i = 0; i < totalSlots; i++)
        {
            if (items[i].itemObject == item)
            {
                items[i].itemObject = null;
                items[i].GetComponent<Image>().sprite = emptySprite;
            }
        }
        Destroy(item.gameObject);
    }
    private void ActivateSlot(int slot)
    {
        Image slotImage = transform.GetChild(slot).GetChild(0).GetComponent<Image>();
        slotImage.transform.GetChild(0).GetComponent<Image>().color = Color.green;
        slotImage.sprite = itemSelectedSprite;
    }
    private void DeactivateSlot(int slot)
    {
        Image slotImage = transform.GetChild(slot).GetChild(0).GetComponent<Image>();
        slotImage.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        slotImage.sprite = itemSprite;
    }
}
