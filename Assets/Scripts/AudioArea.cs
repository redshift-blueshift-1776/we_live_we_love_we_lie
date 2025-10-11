using System.Collections;
using UnityEngine;

public class AudioArea : MonoBehaviour
{
    public AudioSource audioSource;
    private bool playerInArea;
    private float counter;
    public float counterResetThreshold = 4f / 3f;
    private float timeSinceLeft;    //time in seconds since player has left the zone
    private float restartSoundThreshold;
    private IEnumerator currCoroutine;
    void Start()
    {
        audioSource.volume = 0;
        playerInArea = false;
        timeSinceLeft = Mathf.Infinity;
        restartSoundThreshold = 5f;
    }
    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter >= counterResetThreshold)
        {
            if (playerInArea)
            {
                startPlaying();
            }
            else
            {
                stopPlaying();
            }
                counter = 0;
        }

        if (!playerInArea)
        {
            timeSinceLeft += Time.deltaTime;
        }
    }

    private void startPlaying() {
        if (timeSinceLeft >= restartSoundThreshold && audioSource.volume < 0.01f)
        {
            audioSource.Stop();
            audioSource.Play();
            timeSinceLeft = 0;
        }
        if (currCoroutine != null)
        {
            StopCoroutine(currCoroutine);
        }
        currCoroutine = fadeInMusic(1, 1);
        StartCoroutine(currCoroutine);
    }

    private void stopPlaying()
    {
        if (currCoroutine != null)
        {
            StopCoroutine(currCoroutine);
        }
        currCoroutine = fadeInMusic(1, 0);
        StartCoroutine(currCoroutine);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInArea = false;
        }
    }

    //direction is 1 if increasing volume else any other integer for decreasing
    private IEnumerator fadeInMusic(float totalTime, int direction)
    {
        float elapsed = 0;
        float duration = totalTime;
        float originalVolume = audioSource.volume;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp(originalVolume + (direction == 1 ? 1 : -1) * elapsed / duration, 0, 1);
            audioSource.volume = t;
            yield return null;
        }
    }

    public bool isInArea()
    {
        return playerInArea;
    }

    public float getVolume()
    {
        return audioSource.volume;
    }
}
