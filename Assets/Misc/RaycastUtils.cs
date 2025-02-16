using UnityEngine;

public class RaycastTool : MonoBehaviour
{
    public static bool RaycastFromCamera(Camera camera, out RaycastHit hit, float maxDistance = 100f, LayerMask layerMask = default)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }

    public static bool RaycastFromPosition(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance = 100f, LayerMask layerMask = default)
    {
        return Physics.Raycast(origin, direction, out hit, maxDistance, layerMask);
    }
}
