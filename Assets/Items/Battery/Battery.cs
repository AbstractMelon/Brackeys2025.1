using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battery : Item
{
    public override void Use()
    {
        base.Use();
        Debug.Log("Battery used: recharging!");
    }
}

