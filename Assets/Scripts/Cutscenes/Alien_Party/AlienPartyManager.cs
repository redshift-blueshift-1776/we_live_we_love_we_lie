using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Linq;


public class AlienPartyManager : MonoBehaviour
{
    [System.Serializable]
    public class AlienPartyPlayer
    {
        public string playerName;

        public GameObject body;

        public int guess;
        public float profit;

        public Color playerColor;

        public bool isUser;
    }

    [Header("Alien Party Visuals")]
    [SerializeField] private Transform locustWalkStart;
    [SerializeField] private Transform locustWalkEnd;
    [SerializeField] private GameObject ufo;
    [SerializeField] private GameObject lightThing;
    [SerializeField] private Transform ufoStartPosition;
    [SerializeField] private float ufoFlightDuration = 10f;
    [SerializeField] private float ufoHoldingTime = 2f;
    public GameObject ufoCamera;
    // [SerializeField] private GameObject otherCamera;
    [SerializeField] private GameObject genericPersonPrefab;
    [SerializeField] private Color zeroZeropyColor;
    [SerializeField] private Color hannahHalfColor;
    [SerializeField] private Color brianBrainColor;
    [SerializeField] private Color rinaRandomColor;
    [SerializeField] private Color bruhAintNoWayColor;

    public List<AlienPartyPlayer> alienPartyPlayers =
        new List<AlienPartyPlayer>();

    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text resultText;

    [Header("Alien Party Playable")]
    [SerializeField] private bool isPlayableRound = false;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Slider numberPicker; // Limited to only 0 to m-1 for this version.
    public bool canPlay = false;

    [Header("Alien Party Parameters")]
    [SerializeField] private int alien_party_m = 10;
    [SerializeField] private int alien_party_k = 5;

    public List<int> alien_party_guesses = new List<int>();
    public float alien_party_median = -1f;
    public float alien_party_target = -1f;
    public List<float> alien_party_profits = new List<float>(); // Will assume that the guesses and profits are in the same order

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform overviewCamPoint;
    [SerializeField] private Transform playerCamPoint;
    [SerializeField] private Transform finalCamPoint;
    [SerializeField] private Transform bombCamPoint;

    [SerializeField] private float cameraMoveDuration = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resultText.text = "";
        infoText.text = "";
        SetupPlayers();

        if (isPlayableRound)
        {
            uiCanvas.SetActive(true);

            numberPicker.minValue = 0;
            numberPicker.maxValue = alien_party_m - 1;

            canPlay = true;

            infoText.text =
                "Choose your landing segment!";
        }
        else
        {
            StartCoroutine(VisualSequence());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnPersonWithColor(Color c)
    {
        GameObject newPerson =
            Instantiate(genericPersonPrefab);

        Renderer[] renderers =
            newPerson.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.material.color = c;
        }

        newPerson.transform.position = locustWalkStart.position;
        newPerson.transform.localScale = new Vector3(5, 5, 5);

        return newPerson;
    }

    void AddPlayer(
        string playerName,
        int guess,
        Color c,
        bool isUser = false
    )
    {
        AlienPartyPlayer p =
            new AlienPartyPlayer();

        p.playerName = playerName;
        p.guess = guess;
        p.playerColor = c;
        p.isUser = isUser;

        p.body = SpawnPersonWithColor(c);

        alienPartyPlayers.Add(p);
    }

    void SetupPlayers()
    {
        AddPlayer(
            "Zero Zeropy",
            0,
            zeroZeropyColor
        );

        AddPlayer(
            "Hannah Half",
            alien_party_m / 2,
            hannahHalfColor
        );

        int brianBrainGuess =
            Mathf.Clamp(
                (int)(
                    0.77592873f * alien_party_k
                    + 0.99049305f * alien_party_m
                    - 0.65541429f
                ),
                0,
                alien_party_m - 1
            );

        AddPlayer(
            "Brian Brain",
            brianBrainGuess,
            brianBrainColor
        );

        AddPlayer(
            "Rina Random",
            UnityEngine.Random.Range(0, alien_party_m),
            rinaRandomColor
        );

        AddPlayer(
            "Bruh AYNT-noh-WAY",
            (alien_party_m / 2 + alien_party_k) % alien_party_m,
            bruhAintNoWayColor
        );
    }

    // Let user select their number
    public void SetUserGuess()
    {
        if (!canPlay)
        {
            return;
        }

        AddPlayer(
            "You",
            (int)numberPicker.value,
            Color.white,
            true
        );

        canPlay = false;

        uiCanvas.SetActive(false);

        StartCoroutine(VisualSequence());
    }

    // Use alien_party_guesses to calculate the profits
    public void CalculateAndUpdateProfits()
    {
        // Calculate median
        List<int> guesses =
            alienPartyPlayers
            .Select(p => p.guess)
            .OrderBy(g => g)
            .ToList();

        int middle = guesses.Count / 2;

        if (guesses.Count % 2 == 0)
        {
            alien_party_median =
                (guesses[middle - 1]
                + guesses[middle]) / 2f;
        }
        else
        {
            alien_party_median =
                guesses[middle];
        }

        alien_party_target =
            (alien_party_median + alien_party_k)
            % alien_party_m;

        // Calculate losses
        List<float> negLosses = new List<float>();

        foreach (AlienPartyPlayer p in alienPartyPlayers)
        {
            float dist =
                Mathf.Abs(
                    p.guess - alien_party_target
                );

            negLosses.Add(0 - dist);
        }

        // Calculate profits by zero centering
        float avg =
            negLosses.Average();

        for (int i = 0;
            i < alienPartyPlayers.Count;
            i++)
        {
            alienPartyPlayers[i].profit =
                negLosses[i] - avg;
        }
    }

    // ------------
    // Visual Stuff
    // ------------
    IEnumerator MoveCamera(
        Vector3 targetPos,
        Quaternion targetRot,
        float duration
    )
    {
        Vector3 startPos =
            mainCamera.transform.position;

        Quaternion startRot =
            mainCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    elapsed / duration
                );

            mainCamera.transform.position =
                Vector3.Lerp(
                    startPos,
                    targetPos,
                    t
                );

            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    targetRot,
                    t
                );

            yield return null;
        }

        mainCamera.transform.position =
            targetPos;

        mainCamera.transform.rotation =
            targetRot;
}

    IEnumerator MovePlayerToGuess(
        AlienPartyPlayer p
    )
    {
        Vector3 start =
            p.body.transform.position;

        float t =
            (float)p.guess
            / (alien_party_m - 1);

        Vector3 end =
            Vector3.Lerp(
                locustWalkStart.position,
                locustWalkEnd.position,
                t
            );

        float elapsed = 0f;
        float duration = 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            p.body.transform.position =
                Vector3.Lerp(
                    start,
                    end,
                    elapsed / duration
                );

            yield return null;
        }
    }

    IEnumerator FollowUFO(GameObject spawnedUFO, float trackDuration)
    {
        float elapsed = 0f;
        while (spawnedUFO != null && elapsed < trackDuration)
        {
            mainCamera.transform.LookAt(
                spawnedUFO.transform
            );

            if (Vector3.Distance(mainCamera.transform.position, spawnedUFO.transform.position) > 100f)
            {
                mainCamera.transform.position += mainCamera.transform.forward * Time.deltaTime * 125f;
            }

            yield return null;
            elapsed += Time.deltaTime;
        }
        yield return null;
    }

    public IEnumerator VisualSequence()
    {
        infoText.text =
            "Sending signals to aliens...";

        // Spawn lineup
        for (int i = 0;
            i < alienPartyPlayers.Count;
            i++)
        {
            AlienPartyPlayer p =
                alienPartyPlayers[i];

            Vector3 lineupPos =
                locustWalkStart.transform.position
                + Vector3.right * i * 20f;

            p.body.transform.position =
                lineupPos;
        }

        foreach (AlienPartyPlayer p in alienPartyPlayers)
        {
            Vector3 camPos2 =
                p.body.transform.position
                + new Vector3(0, 15, -30);

            Quaternion camRot2 =
                Quaternion.LookRotation(
                    p.body.transform.position - camPos2 + new Vector3(0, 10, 0)
                );

            yield return StartCoroutine(
                MoveCamera(
                    camPos2,
                    camRot2,
                    1.5f
                )
            );

            infoText.text =
                p.playerName
                + "\nGuess: "
                + p.guess;

            yield return new WaitForSeconds(1.5f);
        }

        infoText.text =
            "Claiming land on Locust Walk...";

        yield return StartCoroutine(
            MoveCamera(
                overviewCamPoint.transform.position,
                overviewCamPoint.transform.rotation,
                1.5f
            )
        );

        // Move players
        foreach (AlienPartyPlayer p
            in alienPartyPlayers)
        {
            StartCoroutine(
                MovePlayerToGuess(p)
            );
        }

        yield return new WaitForSeconds(4f);

        // Calculate results
        CalculateAndUpdateProfits();

        infoText.text =
            "Median: "
            + alien_party_median
            + " | Target: "
            + alien_party_target;

        yield return new WaitForSeconds(2f);

        // Spawn UFO
        GameObject spawnedUFO =
            Instantiate(
                ufo,
                ufoStartPosition.position,
                Quaternion.identity
            );

        UFO ufoScript =
            spawnedUFO.GetComponent<UFO>();

        ufoScript.flightDuration = ufoFlightDuration;
        ufoScript.holdingTime = ufoHoldingTime;

        spawnedUFO.transform.position =
            ufoStartPosition.position;

        ufoScript.startPosition = ufoStartPosition;
        float t =
            alien_party_target
            / (alien_party_m - 1);

        Vector3 end =
            Vector3.Lerp(
                locustWalkStart.position,
                locustWalkEnd.position,
                t
            );

        GameObject targetLight = Instantiate(lightThing, transform);
        targetLight.transform.position = end + new Vector3(0, 250, 0);
        
        ufoScript.endPosition = targetLight.transform;

        Vector3 camPos =
            ufoStartPosition.position
            + new Vector3(-80, 40, -80);

        Quaternion camRot =
            Quaternion.LookRotation(
                spawnedUFO.transform.position
                - camPos
            );

        yield return StartCoroutine(
            MoveCamera(
                camPos,
                camRot,
                1f
            )
        );
        
        camRot =
            Quaternion.LookRotation(
                spawnedUFO.transform.position
                - camPos
            );

        yield return StartCoroutine(
            MoveCamera(
                camPos,
                camRot,
                1f
            )
        );

        yield return StartCoroutine(
            FollowUFO(spawnedUFO, ufoFlightDuration)
        );

        Vector3 finalPos =
            end
            + new Vector3(-300, 15, -30);

        Quaternion finalRot =
            Quaternion.LookRotation(
                locustWalkEnd.position
                - finalPos
            );

        yield return StartCoroutine(
            MoveCamera(
                finalPos,
                finalRot,
                2f
            )
        );

        // Show profits
        string resultString = "";

        foreach (AlienPartyPlayer p
            in alienPartyPlayers)
        {
            resultString +=
                p.playerName
                + ": "
                + p.profit.ToString("F2")
                + "\n";
        }

        resultText.text = resultString;
    }
}
