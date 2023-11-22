using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Assign the player's transform to this field in the Inspector
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        // Get the CinemachineVirtualCamera component from the Main Camera
        virtualCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();

        // Check if the virtualCamera is not null
        if (virtualCamera != null)
        {
            // Set the Follow and Look At targets to the player
            virtualCamera.Follow = player;
            virtualCamera.LookAt = player;
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera component not found on the Main Camera.");
        }
    }
}

