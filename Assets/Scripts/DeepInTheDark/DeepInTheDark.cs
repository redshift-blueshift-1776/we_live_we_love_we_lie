using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class DeepInTheDark : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public GameObject deathZone;

    //sound
    public AudioManager audioManager;

    private bool powerOff;


    public PlayerMovement playerMovement;

    private Vector3 initialPlayerPosition;
    private float deathZoneY;

    private bool gameActive;
    public float timer;

    
    private Color spectreColor = new Color(127f / 255f, 224f / 255f, 255f / 255f);
    private float fadeTimer = 0.25f;

    private float initialLightningDelay = 5f;

    private float intervalBetweenLightning = 3f;

    [SerializeField] private TMP_Text timerText;


    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject EndScreenCanvas;
    [SerializeField] private GameObject winText;

    void Start()
    {
        powerOff = false;
        audioManager.setStartTime("lightning", 0.1f);
        audioManager.setStartTime("machineSpark", 3.3f);
        //flashLightning();
        initialPlayerPosition = player.transform.position;
        deathZoneY = deathZone.transform.position.y;
        StartCoroutine(powerOffSounds());
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
            killPlayer();
        }
    }

    private void killPlayer()
    {
        //TO-DO add checkpoints
        playerMovement.haltMovement();
        player.transform.position = initialPlayerPosition;
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
        StartCoroutine(fadeSkyboxColor());
    }

    private IEnumerator powerOffSounds()
    {
        yield return new WaitForSeconds(initialLightningDelay -  1.9f);
        audioManager.playSound("machineSpark");
        yield return new WaitForSeconds(1.5f);
        audioManager.playSound("powerOff");
    }

    private IEnumerator fadeSkyboxColor()
    {
        if (powerOff)
        {
            audioManager.playSound("lightning");
        }
            //allow time for sound to start playing

            float duration = fadeTimer;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            if (elapsed < 0.25 * duration && powerOff)
            {
                RenderSettings.ambientSkyColor = Color.Lerp(Color.black, spectreColor, t);
            } else
            {
                RenderSettings.ambientSkyColor = Color.Lerp(spectreColor, Color.black, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        powerOff = true;
        RenderSettings.ambientSkyColor = Color.black;
    }
}
