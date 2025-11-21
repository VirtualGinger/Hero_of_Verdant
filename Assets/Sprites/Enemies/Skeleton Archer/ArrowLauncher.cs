using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    public Transform launchPoint;       
    public GameObject projectilePrefab;
    public SpriteRenderer spriteRef;    

    
    public void FireProjectile(Vector2 direction)
    {
        // Spawn arrow at launchPoint
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);

       
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Pass direction to arrow
        Arrow arrow = projectile.GetComponent<Arrow>();
        if (arrow != null)
        {
            arrow.SetDirection(direction);
        }
    }
}