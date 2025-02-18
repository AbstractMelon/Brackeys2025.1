using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bonfire : Item
{
    public override void Use()
    {
        Debug.Log("Bonfire used! Lighting a bonfire...");
    }

    public override Quaternion DefaultRotation()
    {
        return Quaternion.Euler(0f, 180f, 0f);
    }
}

