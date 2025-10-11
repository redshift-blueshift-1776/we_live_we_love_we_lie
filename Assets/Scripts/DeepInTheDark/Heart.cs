using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    private bool collected;

    public GameObject heart;
    public AudioManager audioManager;
    public PlayerMovement playerMovement;
    public Canvas endScreenCanvas;
    public Image backgroundFilter;
    public TMP_Text endScreenText;
    public Image blackScreen;
    public GameObject crosshair;
    public GameSettings gameSettings;
    public SceneChanger sceneChanger;

    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    void Start()
    {
        collected = false;

        endScreenCanvas.gameObject.SetActive(false);
        backgroundFilter.gameObject.SetActive(false);
        endScreenText.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);

        meshCollider = heart.GetComponent<MeshCollider>();
        meshRenderer = heart.GetComponent<MeshRenderer>();

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

        audioManager.setVolume("collect", 0.4f);
        audioManager.setVolume("collectLoop", 0.4f);
        audioManager.setIsLooping("collectLoop", true);

        audioManager.setVolume("collectOutro", 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private const float verticalDisplacementAmplitude = 0.5f;
    private const float verticalDisplacementPeriod = 2.0f;
    private const float rotationPeriod = 0.25f;  //make sure this is less than secondsBetweenRotate
    private const float secondsBetweenRotate = 2.0f;
    private const float secondsBetweenHeartPulse = 2f;
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

    private const float movementSlowdownTime = 5.0f;
    private const float minTimeScaleThreshold = 0.01f;
    private IEnumerator collect()
    {
        collected = true;
        meshCollider.isTrigger = true;
        meshRenderer.enabled = false;
        gameSettings.setGameEnded(true);

        StartCoroutine(startTransition());
        float t = 0;
        float exponentialDecayBase = Mathf.Pow(minTimeScaleThreshold, -1 / movementSlowdownTime);
        while (true)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Pow(exponentialDecayBase, -t);
            if (Time.timeScale <= minTimeScaleThreshold)
            {
                break;
            }
            yield return null;
        }
        Time.timeScale = 0;

        yield return null;
    }

    private IEnumerator startTransition()
    {
        audioManager.playSound("collect");
        StartCoroutine(showEndScreenCanvas());
        float collectSoundLength = audioManager.getLength("collect");

        yield return new WaitForSecondsRealtime(collectSoundLength - 4 * Time.unscaledDeltaTime);

        audioManager.playSound("collectLoop");
        yield return null;
    }

    private IEnumerator showEndScreenCanvas()
    {
        endScreenCanvas.gameObject.SetActive(true);
        backgroundFilter.gameObject.SetActive(true);
        crosshair.gameObject.SetActive(false);
        float backgroundBrightness = 1f;
        float textAlpha = 0f;
        float backgroundAlpha = 0.75f;

        const float backgroundBrightnessChangeDuration = 2f;
        const float textAlphaChangeDuration = 2f;
        float t = 0;

        while (t < backgroundBrightnessChangeDuration)
        {
            t += Time.unscaledDeltaTime;
            backgroundBrightness = 1 - t / backgroundBrightnessChangeDuration;
            backgroundFilter.color = new Color(backgroundBrightness, backgroundBrightness, backgroundBrightness, backgroundAlpha);
            yield return null;
        }
        backgroundFilter.color = new Color(0, 0, 0, backgroundAlpha);

        endScreenText.gameObject.SetActive(true);

        t = 0;
        Color color = endScreenText.color;
        while (t < textAlphaChangeDuration)
        {
            t += Time.unscaledDeltaTime;
            textAlpha = t / textAlphaChangeDuration;
            endScreenText.color = new Color(color.r, color.g, color.b, textAlpha);
            yield return null;
        }
        endScreenText.color = new Color(color.r, color.g, color.b, 1f);

        yield return new WaitForSecondsRealtime(1);
        while (true)
        {
            if (Input.anyKeyDown)
            {
                break;
            }
            yield return null;
        }
        StartCoroutine(fadeOutCanvas());
        yield return null;
    }

    private IEnumerator fadeOutCanvas()
    {
        blackScreen.gameObject.SetActive(true);

        audioManager.stopSound("collectLoop");
        audioManager.playSound("collectOutro");

        float fadeDuration = audioManager.getLength("collectOutro");
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = t / fadeDuration;
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = Color.black;

        yield return new WaitForSecondsRealtime(1f);
        sceneChanger.LoadSceneByNumber(7);

        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                StartCoroutine(collect());
            }
            else
            {
                Vector3 currVelocity = playerMovement.getPlayerVelocity();
                float vx = currVelocity.x;
                float vy = currVelocity.y;
                float vz = currVelocity.z;
                playerMovement.setPlayerVelocity(new Vector3(-vx, vy, -vz));
                audioManager.playSound($"bounce{Random.Range(0, 4)}");
            }
        }
    }
}
