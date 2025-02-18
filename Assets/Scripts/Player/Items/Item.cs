using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    //Modify this a bit to allow for inheriting, and just change the texture and what happens during usage
    [SerializeField] private Sprite inventoryTexture;
    public InventoryItem item;
    public bool isHeld { get; private set; }
    private Color[] defaultColors;
    private new Renderer renderer;
    void Awake()
    {
        renderer = GetComponent<Renderer>();
        defaultColors = new Color[renderer.materials.Length];
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            defaultColors[i] = renderer.materials[i].color;
        }
    }
    public void Pickup()
    {
        isHeld = true;
        item.GetComponent<Image>().sprite = inventoryTexture;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        transform.localRotation = DefaultRotation();
        transform.localPosition = Vector3.zero;
    }
    public virtual void Use()
    {
        Debug.Log("Item used");
    }
    public void Hold()
    {
        gameObject.SetActive(true);
        transform.localRotation = DefaultRotation();

    }
    public virtual Quaternion DefaultRotation()
    {
        return Quaternion.identity;
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
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            renderer.materials[i].color = Color.green;
        }
    }
    public void Unhighlight()
    {
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            renderer.materials[i].color = defaultColors[i];
        }
    }
}


