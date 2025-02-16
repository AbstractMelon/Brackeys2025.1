using UnityEngine;

public class ScreenWrapper2D : MonoBehaviour
{
    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        
        if(viewportPos.x > 1) transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0, viewportPos.y, 0));
        if(viewportPos.x < 0) transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1, viewportPos.y, 0));
    }
}