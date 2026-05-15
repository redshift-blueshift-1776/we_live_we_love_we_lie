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
    [SerializeField] private GameObject otherCamera;
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
                + Vector3.forward * i * 10f;

            p.body.transform.position =
                lineupPos;
        }

        yield return new WaitForSeconds(2f);

        infoText.text =
            "Claiming land on Locust Walk...";

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

        yield return new WaitForSeconds(
            ufoFlightDuration
            + ufoHoldingTime
            + 2f
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
