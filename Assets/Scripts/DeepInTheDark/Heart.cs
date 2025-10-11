using System.Collections;
using UnityEngine;

public class Heart : MonoBehaviour
{
    private bool collected;
    public GameObject heart;
    public AudioManager audioManager;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private const float verticalDisplacementAmplitude = 0.5f;
    private const float verticalDisplacementPeriod = 2.0f;

    private const float rotationPeriod = 0.25f;  //make sure this is less than secondsBetweenRotate
    private const float secondsBetweenRotate = 2.0f;

    private const float secondsBetweenHeartPulse = 2f;
    void Start()
    {
        collected = false;
        originalPosition = heart.transform.position;
        originalRotation = heart.transform.rotation;

        initializeSounds();
        StartCoroutine(idle());
    }

    private void initializeSounds()
    {
        audioManager.setVolume("pulse", 0.5f);
        audioManager.setSpatialBlend("pulse", 1);
        audioManager.setDopplerLevel("pulse", 0);
        audioManager.setVolumeRolloffMode("pulse", "Linear");
        audioManager.setMinDistance("pulse", 0);
        audioManager.setMaxDistance("pulse", 50);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator idle()
    {
        float positionTime = 0;
        float rotateTime = 0;
        float pulseTime = 0;
        while (true)
        {
            if (collected)
            {
                yield break;
            }
            
            heart.transform.position = originalPosition + new Vector3(
                0, 
                verticalDisplacementAmplitude * Mathf.Sin(2 * Mathf.PI / verticalDisplacementPeriod * positionTime), 
                0
            );

            if (rotateTime >= secondsBetweenRotate)
            {
                StartCoroutine(rotate());
                rotateTime = 0;
            }

            if (pulseTime >= secondsBetweenHeartPulse)
            {
                pulseTime = 0;
            }

            if (pulseTime == 0)
            {
                audioManager.playSound("pulse");
            }

            positionTime += Time.deltaTime;
            rotateTime += Time.deltaTime;
            pulseTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator rotate()
    {
        float t = 0;
        Quaternion endRotation = Quaternion.Euler(0, 180f, 0) * originalRotation;

        while (t < rotationPeriod)
        {
            t += Time.deltaTime;
            float normalizedT = Mathf.Clamp01(t / rotationPeriod);
            heart.transform.rotation = Quaternion.Slerp(originalRotation, endRotation, normalizedT);
            yield return null;
        }
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
