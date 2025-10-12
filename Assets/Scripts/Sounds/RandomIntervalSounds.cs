using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtRandomIntervals : MonoBehaviour
{
    public float minSeconds = 5f;
    public float maxSeconds = 15f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound()
    {
        while (true)
        {
            float waitTime = Random.Range(minSeconds, maxSeconds);
            yield return new WaitForSeconds(waitTime);
            audioSource.Play();
        }
    }
}