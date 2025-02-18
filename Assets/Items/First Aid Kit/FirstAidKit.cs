using UnityEngine;

public class FirstAidKit : Item
{
    public int healAmount;
    public override void Use()
    {
        transform.parent.parent.GetComponent<HealthSystem>().Heal(healAmount);
        Delete();
    }
    public override Quaternion DefaultRotation()
    {
        return Quaternion.Euler(-90f, 0f, 0f);
    }
}
