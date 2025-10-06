using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float transitionSpeed = 5f;
    public Transform playerTarget;

    private Vector3 targetPosition;
    private bool isFollowing = false;
    private Coroutine transitionCoroutine;


    void Start()
    {

    }

    void LateUpdate()
    {
        if (isFollowing && playerTarget != null)
        {
            Vector3 desiredPosition = new Vector3(playerTarget.position.x, playerTarget.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * transitionSpeed);
        }
    }

    public void MoveToNewRoom(Vector3 newPosition)
    {
        isFollowing = false;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        targetPosition = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        transitionCoroutine = StartCoroutine(TransitionTo(targetPosition));
    }

    public void StartFollowingPlayer()
    {
        isFollowing = true;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
    }

    private IEnumerator TransitionTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * transitionSpeed);
            yield return null;
        }

        transform.position = target;
        transitionCoroutine = null;
    }
}