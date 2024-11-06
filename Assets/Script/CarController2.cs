using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    public float moveSpeed = 10f;         // Speed of the car moving forward
    public float stopDuration = 3f;       // Time to stop at the stop sign
    public float detectionRange = 15f;    // Range within which the stop sign will be detected

    private bool isStopped = false;       // Flag to check if the car is stopped
    private bool stopTriggered = false;   // Flag to prevent multiple stops

    public Camera firstPersonCamera;      // Reference to the first-person camera

    void Update()
    {
        if (!isStopped)
        {
            MoveForward();
            DetectStopSign();
        }
    }

    // Function to move the car forward continuously
    void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    // Function to detect the stop sign using raycasting from the camera
    void DetectStopSign()
    {
        RaycastHit hit;

        // Cast a ray from the camera's position forward
        if (Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("StopSign") && !stopTriggered)
            {
                StartCoroutine(StopAtStopSign());
            }
        }
    }

    // Coroutine to stop the car for a duration and then resume moving forward
    IEnumerator StopAtStopSign()
    {
        stopTriggered = true;  // Prevent re-triggering stop
        isStopped = true;      // Stop the car's movement
        Debug.Log("Stop sign detected, stopping...");

        yield return new WaitForSeconds(stopDuration);  // Wait at the stop sign

        isStopped = false;      // Resume movement after stopping
        stopTriggered = false;  // Reset the trigger so the car can stop again if needed
        Debug.Log("Resuming...");
    }
}

