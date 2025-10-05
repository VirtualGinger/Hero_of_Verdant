using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Transform cameraPosition;
    public string zoneName;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.MoveToNewRoom(cameraPosition.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.StartFollowingPlayer();
            }
        }
    }
}