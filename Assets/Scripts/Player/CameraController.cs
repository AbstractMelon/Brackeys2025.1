using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private PlayerController player;
    private float baseRotationY;
    private Vector3 rotationVariance;
    void Awake()
    {
        cam = GetComponent<Camera>();
        player = transform.parent.GetComponent<PlayerController>();
        baseRotationY = 0f;
        rotationVariance = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        bool walking = Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0;
        bool sprinting = Input.GetKeyDown(KeyCode.LeftShift) && walking;
        baseRotationY = ClampAngle(baseRotationY - Input.GetAxis("Mouse Y") * player.mouseSensitivity);
        /*
        if (walking)
        {
            Modify the rotation variance based on if the player is walking and/or sprinting
        }
        */
        cam.transform.localEulerAngles = new Vector3(baseRotationY, 0f, 0f) + rotationVariance;

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
