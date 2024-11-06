using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    void Start()
    {
        if (Display.displays.Length > 1)
        {
            // Activate the second display for the third-person camera
            Display.displays[1].Activate();
            firstPersonCamera.targetDisplay = 0; // Display 1
            thirdPersonCamera.targetDisplay = 1; // Display 2
        }
    }
}
