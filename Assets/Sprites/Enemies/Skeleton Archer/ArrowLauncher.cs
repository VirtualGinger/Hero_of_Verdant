using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectilePrefab;

    public SpriteRenderer spriteRef; // assign the enemy's SpriteRenderer here

    public void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);

        bool facingRight = true;
        if (spriteRef != null) facingRight = !spriteRef.flipX;
        else facingRight = transform.localScale.x >= 0;

        float facing = facingRight ? 1f : -1f;

        Vector3 origScale = projectile.transform.localScale;
        projectile.transform.localScale = new Vector3(origScale.x * facing, origScale.y, origScale.z);
        projectile.transform.rotation = facingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
    }
}
