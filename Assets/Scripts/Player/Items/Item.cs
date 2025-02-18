using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    //Modify this a bit to allow for inheriting, and just change the texture and what happens during usage
    public Sprite inventoryTexture;
    public InventoryItem item;
    public bool isHeld;
    public void Pickup()
    {
        isHeld = true;
        item.GetComponent<Image>().sprite = inventoryTexture;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }
    public virtual void Use()
    {
        Debug.Log("Item used");
    }
    public void Hold()
    {
        gameObject.SetActive(true);
    }
    public void StopHolding()
    {
        gameObject.SetActive(false);
    }
    public void Drop()
    {
        gameObject.SetActive(true);
        isHeld = false;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.SetParent(null);
    }
    public void Delete()
    {
        item.inventory.DestroyItem(this);
    }

    public void Highlight()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }
    public void Unhighlight()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}


