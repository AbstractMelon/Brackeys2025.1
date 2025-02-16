using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private PlayerController player;
    void Awake()
    {
        cam = GetComponent<Camera>();
        player = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.localEulerAngles = new Vector3(ClampAngle(cam.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * player.mouseSensitivity), cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
    }
        public static float ClampAngle(float angle) {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        if (angle > 90)
            angle = 90;
        if (angle < -90)
            angle = -90;

        return angle;
    }
}
