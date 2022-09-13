using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRotation : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    public Transform playerBody;
    public Transform gunHold;
    public Transform gun;


    private float xRotation = 0f;
    private float yRotation = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
       // yRotation = Mathf.Clamp(yRotation, -180, 180);


        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        //gunHold.Rotate(Vector3.up * mouseX);
        gun.Rotate(Vector3.up * mouseX);
    }
}
