using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectilePrefab;

    public void FireProjectile()
    {
        Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
    }
}
