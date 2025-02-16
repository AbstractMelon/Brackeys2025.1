using UnityEngine;

public class ProjectileShooter2D : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float shootForce = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject projectile = Instantiate(projectilePrefab, 
                transform.position, 
                Quaternion.identity);
            
            projectile.GetComponent<Rigidbody2D>()
                .AddForce(Vector2.right * shootForce, ForceMode2D.Impulse);
        }
    }
}