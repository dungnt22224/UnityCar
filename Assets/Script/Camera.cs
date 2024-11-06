using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    public float moveSmoothness = 0.125f;   // Speed of camera position smoothing
    public float rotSmoothness = 0.125f;    // Speed of camera rotation smoothing

    public Vector3 moveOffset;   // Offset for the camera position relative to the car
    public Vector3 rotOffset;    // Offset for the camera rotation relative to the car

    public Transform carTarget;  // The car the camera will follow

    void FixedUpdate()   // FixedUpdate for consistent physics-based updates
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // Calculate the target position based on the car's position and the offset
        Vector3 targetPos = carTarget.TransformPoint(moveOffset);

        // Smoothly interpolate camera position towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        // Calculate the target rotation (car's rotation with additional rotation offset)
        Quaternion targetRot = carTarget.rotation * Quaternion.Euler(rotOffset);

        // Smoothly interpolate the camera rotation to match the car's rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSmoothness * Time.deltaTime);
    }
}
