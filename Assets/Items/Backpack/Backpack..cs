using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : Item
{
    public int carryCapacity = 10;

    private List<Item> items = new List<Item>();

    public override void Use()
    {
        if(items.Count < carryCapacity)
        {
            var item = RaycastForItem();
            if(item != null)
            {
                AddItem(item);
            }
        }
        else
        {
            var item = items[0];
            RemoveItem(item);
        }
    }

    private Item RaycastForItem()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            var item = hit.transform.GetComponent<Item>();
            if (item != null)
                return item;
        }
        return null;
    }

    private void AddItem(Item item)
    {
        items.Add(item);
        item.Pickup();
        item.transform.parent = transform;
    }

    private void RemoveItem(Item item)
    {
        items.Remove(item);
        item.Drop();
    }
}
