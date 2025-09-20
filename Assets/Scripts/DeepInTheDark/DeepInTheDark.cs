using UnityEngine;
using TMPro;
using System.Collections;

public class DeepInTheDark : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public GameObject deathZone;
    public AudioSource lightningSound;
    public PlayerMovement playerMovement;

    private Vector3 initialPlayerPosition;
    private float deathZoneY;

    private bool gameActive;
    public float timer;

    
    private Color spectreColor = new Color(127 / 255, 224 / 255, 255 / 255);
    private float fadeTimer = 0.5f;
    private float initialLightningDelay = 2f;
    private float intervalBetweenLightning = 3f;

    [SerializeField] private TMP_Text timerText;


    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject EndScreenCanvas;
    [SerializeField] private GameObject winText;

    void Start()
    {
        //flashLightning();
        initialPlayerPosition = player.transform.position;
        deathZoneY = deathZone.transform.position.y;
        InvokeRepeating("flashLightning", initialLightningDelay, intervalBetweenLightning);
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
        handleDeathZone();
    }

    private void handleDeathZone()
    {
        if (player.transform.position.y <= deathZoneY)
        {

            player.transform.position = initialPlayerPosition;
            playerMovement.haltMovement();
        }
    }

    private void updateTimer()
    {
        if (gameActive)
        {
            timerText.text = $"Time Remaining: {120 - Mathf.Floor(timer)}";
            timer += Time.deltaTime;
            if (timer > 120f)
            {
                //StartCoroutine(EndGame());
            }
        }
    }

    void flashLightning()
    {
        lightningSound.Play();
        StartCoroutine(fadeSkyboxColor());
    }

    private IEnumerator fadeSkyboxColor()
    {
        float duration = fadeTimer;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            RenderSettings.ambientSkyColor = Color.Lerp(spectreColor, Color.black, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        RenderSettings.ambientSkyColor = Color.black;
    }
}
