using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraLook : MonoBehaviour
{
    // Reference to the players camera transform.
    public Camera CameraPlayer;

    // values for mouse input.
    public Vector2 Sens;

    // Variable to store the current rotationforthe x and y axes.
    private Vector2 XYRotation;

  
    void Start()
    {
        // For locking the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

   
    void Update()
    {
        Camera();
    }
    public void Camera()
    {
        //  mouse input for rotation.
        Vector2 MouseInput = new Vector2
        {

            x = Input.GetAxisRaw("Mouse X"),
            y = Input.GetAxisRaw("Mouse Y")
        };

        // Adjust the current rotation based on mouse input.
        XYRotation.x -= MouseInput.y * Sens.y; // Invert Y-axis for camera rotation.
        XYRotation.y += MouseInput.x * Sens.x;

        // Clamp to to prevent the camera from flipping/getting stuck like goutham.
        XYRotation.x = Mathf.Clamp(XYRotation.x, -90f, 90f);

        // Update the player's rotation around the Y-axis.
        transform.eulerAngles = new Vector3(0f, XYRotation.y, 0);

        // Updates the cameras local rotation around the X-axis.
        CameraPlayer.transform.localEulerAngles = new Vector3(XYRotation.x, 0, 0);
    }
}
