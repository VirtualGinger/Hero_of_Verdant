using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public enum RoomType { Static, Follow }

    public RoomType roomType;
    public Transform cameraPosition;

    private CameraController mainCamera;

    void Start()
    {
        mainCamera = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (roomType == RoomType.Static)
            {
                mainCamera.MoveToNewRoom(cameraPosition.position);
            }
            else if (roomType == RoomType.Follow)
            {
                mainCamera.StartFollowingPlayer();
            }
        }
    }
}