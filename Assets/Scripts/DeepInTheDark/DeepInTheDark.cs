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
    private float fadeTimer = 5f;

    //this must be at least 1.9 (preferably 2).
    private float initialLightningDelay = 5f;

    private float intervalBetweenLightning = 10f;

    [SerializeField] private TMP_Text timerText;


    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject EndScreenCanvas;
    [SerializeField] private GameObject winText;

    void Start()
    {
        powerOff = false;

        RenderSettings.ambientSkyColor = spectreColor;
        initializeStartTimes();
        
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

    private void initializeStartTimes()
    {
        audioManager.setStartTime("lightning", 0.1f);
        audioManager.setStartTime("machineSpark", 3.3f);
        audioManager.setStartTime("respawn", 0.1f);
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
        playerMovement.haltMovement();
        player.transform.position = initialPlayerPosition;
        audioManager.playSound("respawn");
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
            //allow time for sound to start playing

        float duration = fadeTimer;
        float elapsed = 0f;
        bool lightningSoundPlayed = false;

        float peakColorPercentTime = 0.025f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            if (powerOff)
            {
                if (elapsed < peakColorPercentTime * duration)
                {
                    //4t because 1/25% = 4
                    RenderSettings.ambientSkyColor = Color.Lerp(Color.black, spectreColor, 4 * t);

                    //play sound slightly before
                    if (t >= peakColorPercentTime - 0.15f / fadeTimer && !lightningSoundPlayed)
                    {
                        audioManager.playSound("lightning");
                        lightningSoundPlayed = true;
                    }

                }
                else
                {
                    RenderSettings.ambientSkyColor = Color.Lerp(spectreColor, Color.black, 4 * t / 3);
                }
            }
            else
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
