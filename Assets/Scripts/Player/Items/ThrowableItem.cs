using UnityEngine;

public class ThrowableItem : Item
{
    public float throwPower;
    public int damage;
    public int playerLayer = 7;
    public GameObject holdingPlayer;
    public override void Use()
    {
        holdingPlayer = transform.parent.parent.gameObject;
        Vector3 directionalMult = holdingPlayer.transform.GetChild(0).rotation * new Vector3(1f, 1f, throwPower);
        item.inventory.DiscardHeldItem();
        GetComponent<Rigidbody>().AddForce(directionalMult, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == playerLayer && collision.gameObject != holdingPlayer)
        {
            collision.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
            Delete();
        }
    }
}
