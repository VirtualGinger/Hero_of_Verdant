using UnityEngine;
using UnityEngine.UI;

public class PlayerPower : MonoBehaviour
{
    public float power;
    public float maxPower;
    public Image PowerBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxPower = 100f;
        power = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        PowerBar.fillAmount = Mathf.Clamp(power / maxPower, 0, 1);

    }

}
