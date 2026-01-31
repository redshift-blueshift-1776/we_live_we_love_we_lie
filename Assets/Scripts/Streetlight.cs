using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Streetlight : MonoBehaviour
{
    public int displacement;

    public float secondsPerBeat;

    public float tempo;

    public double nextChangeTime;

    public int lastColorBeat = -1;

    private Color emissionColor;
    Renderer targetRenderer;

    private Coroutine currentFlashCoroutine;
    [SerializeField] public GameObject lightCube;
    private Light lightSource;
    [SerializeField] private float maxLightSourceRange = 30f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextChangeTime = BeatManager.Instance.GetNextBeatTime();
        targetRenderer = lightCube.GetComponent<Renderer>();
        emissionColor = targetRenderer.material.GetColor("_EmissionColor");
        lightSource = lightCube.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!BeatManager.Instance.audioSource.isPlaying) return;

        int currentBeat = BeatManager.Instance.GetCurrentBeatNumber();

        if (currentBeat != lastColorBeat)
        {
            // Debug.Log("New Color Beat");
            lastColorBeat = currentBeat;
            if (currentBeat % 4 == displacement) {
                // Debug.Log("Flashing Color");
                if (currentFlashCoroutine != null)
                {
                    StopCoroutine(currentFlashCoroutine);
                }

                currentFlashCoroutine = StartCoroutine(FlashColor());
            }
        }
    }

    public IEnumerator FlashColor() {
        float elapsed = 0f;
        float duration = 3 * (float)BeatManager.Instance.GetSecondsPerBeat();
        

        float maxChannel = Mathf.Max(emissionColor.r, emissionColor.g, emissionColor.b);
        Color normalizedColor = emissionColor / maxChannel;

        Color.RGBToHSV(normalizedColor, out float h, out float s, out float v);

        while (elapsed < duration)
        {
            float t = elapsed / duration;


            float newIntensity = Mathf.Lerp(maxChannel, 0f, t);
            Color newColor = Color.HSVToRGB(h, s, Mathf.Lerp(v, 0, t)) * newIntensity;

            targetRenderer.material.EnableKeyword("_EMISSION");
            targetRenderer.material.SetColor("_EmissionColor", newColor);
            //material.SetColor("_BaseColor", Color.Lerp(baseColor, Color.black, t));

            //Debug.Log($"Intensity: {newIntensity}, Color: {newColor}");
            lightSource.range = Mathf.Lerp(maxLightSourceRange, 0, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        targetRenderer.material.SetColor("_EmissionColor", emissionColor);
        yield return null;
    }
}
