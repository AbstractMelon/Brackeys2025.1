using UnityEngine;

public class ProjectileShooter3D : MonoBehaviour
{
    [Tooltip("Prefab to shoot")]
    public GameObject projectilePrefab;
    [Tooltip("Shoot force")]
    public float shootForce = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject projectile = Instantiate(projectilePrefab, 
                transform.position, 
                Quaternion.identity);
            
            projectile.GetComponent<Rigidbody>()
                .AddForce(Camera.main.transform.forward * shootForce, ForceMode.Impulse);
        }
    }
}