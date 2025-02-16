using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player;
    private new Camera camera;
    [SerializeField]
    private float mouseSensitivity;
    void Awake()
    {
        camera = Camera.main;
player = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
        transform.Rotate(new Vector3(mouseSensitivity * Input.GetAxis("Mouse X"), 0, 0));

    }
}