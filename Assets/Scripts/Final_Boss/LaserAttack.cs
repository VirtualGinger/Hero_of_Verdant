using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    [Header("Laser Settings")]
    public Transform firePoint;            // where the laser originates
    public GameObject laserBeamPrefab;     // prefab of the laser shot
    public float laserDuration = 0.5f;     // how long the laser exists

    // Call this to fire the laser
    public void FireLaser(Vector2 direction)
    {
        // Spawn the laser beam at firePoint
        GameObject laser = Instantiate(laserBeamPrefab, firePoint.position, Quaternion.identity);

        // Rotate laser to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Send direction to the laser beam script (if needed)
        LaserBeam beam = laser.GetComponent<LaserBeam>();
        if (beam != null)
        {
            beam.SetDirection(direction);
        }

        // Destroy laser after duration
        Destroy(laser, laserDuration);
    }
}