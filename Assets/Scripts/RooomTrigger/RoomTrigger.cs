using UnityEngine;
using UnityEditor;

public class RoomTrigger : MonoBehaviour
{
    // We no longer need the enum, it's always a static trigger.
    public Transform cameraPosition;

    private CameraController mainCamera;

    public string zoneName;

    void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mainCamera.MoveToNewRoom(cameraPosition.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mainCamera.StartFollowingPlayer();
        }
    }
}