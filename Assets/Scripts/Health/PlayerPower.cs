using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPower : MonoBehaviour
{
    public float power;
    public float maxPower;
    public Image PowerBar;

    // Flag for invulnerability
    public bool isInvulnerable = false;

    private void Start()
    {
        maxPower = 100f;
        power = 20f;
    }

    private void Update()
    {
        PowerBar.fillAmount = Mathf.Clamp(power / maxPower, 0, 1);

        // If power reaches max, trigger invulnerability once
        if (!isInvulnerable && power >= maxPower)
        {
            StartCoroutine(ActivateInvulnerability());
        }
    }

    private IEnumerator ActivateInvulnerability()
    {
        isInvulnerable = true;

        float duration = 15f;
        float elapsed = 0f;
        float startPower = maxPower;

        // Drain power smoothly over 60 seconds
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            power = Mathf.Lerp(startPower, 0f, elapsed / duration);
            yield return null;
        }

        // End invulnerability
        power = 0f;
        isInvulnerable = false;
    }
}
