using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Universal_Manager : MonoBehaviour
{
    [SerializeField] public bool debug;

    [Header("Statistics Needed")]
    [SerializeField] public int numLevels; // Story Mode
    [SerializeField] public int numHardLevels;
    [SerializeField] public int numNonInfiniteLevels;

    [SerializeField] public int numOpenExplorationCollectibles;
    [SerializeField] public int numOpenExplorationLevels;

    [Header("Unlocked Levels")]
    public bool[] unlockedEasy;
    public bool[] unlockedHard;
    public bool[] unlockedEndless;
    

    [Header("Overall Achievements")]
    public bool[] beatStoryModeLevels;
    public bool beatStoryMode;
    public bool beatStoryModeWithoutFailing;
    public bool[] beatHardLevels;
    public bool[] beatNonInfiniteLevels;

    [Header("Speedrun Achievements")]
    public bool level1Speedrun;
    public bool level2Speedrun;
    public bool level3Speedrun;
    public bool level4Speedrun;
    public bool level5Speedrun;
    public bool level5Speedrun2;
    public bool level6Speedrun;
    public bool level7Speedrun;

    [Header("Level 1 Achievements")]
    public bool level1Layer20;
    public bool level1Layer20noWSUpDown;
    public bool level1Layer20DLeft;
    public bool level1Layer50;
    public bool level1Layer100;

    [Header("Level 2 Achievements")]
    public bool level2iteration5;
    public bool level2iteration10;
    public bool level2iteration5noGrapple;
    public bool level2iteration10noGrapple;

    [Header("Level 3 Achievements")]
    public bool level3iteration5;

    [Header("Level 4 Achievements")]
    public bool level4GetBetrayed;

    [Header("Level 5 Achievements")]

    [Header("Level 8 Achievements")]
    public bool level8Get1500;
    public bool level8Get2000;

    [Header("For Final Elimination")]
    public bool justBeatLevel8;

    [Header("Open Exploration")]
    public bool[] openExplorationCollectibles;
    public int numCollectiblesCollected;
    public bool openExplorationCollect10Collectibles;
    public bool openExplorationCollect20Collectibles;
    public bool[] openExplorationBallChallenges;

    [Header("Camera Effects")]
    public int usePostProcessing;
    [SerializeField] public RenderTexture defaultRenderTexture;
    [SerializeField] public RenderTexture cameraEffect;
    [SerializeField] public GameObject defaultRawImage;
    [SerializeField] public GameObject cameraEffectRawImage;

    public bool skipTransitions = false;

    public bool mobileSuperCheat = false;

    public static Universal_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist this instance
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unlockedEasy = new bool[8] {
            true, true, true, true, true, true, true, true
        };
        unlockedHard = new bool[8];
        unlockedEndless = new bool[8];
        beatStoryModeLevels = new bool[numLevels];
        beatHardLevels = new bool[numHardLevels];
        beatNonInfiniteLevels = new bool[numNonInfiniteLevels];
        openExplorationCollectibles = new bool[numOpenExplorationCollectibles];
        numCollectiblesCollected = 0;
        openExplorationBallChallenges = new bool[numOpenExplorationLevels];
        defaultRawImage.SetActive(false);
        cameraEffectRawImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        level1Layer20 = (PlayerPrefs.GetInt("level1Layer20", 0) == 1);
        level1Layer50 = (PlayerPrefs.GetInt("level1Layer50", 0) == 1);
        level1Layer100 = (PlayerPrefs.GetInt("level1Layer100", 0) == 1);

        level2iteration5 = (PlayerPrefs.GetInt("level2iteration5", 0) == 1);
        level2iteration10 = (PlayerPrefs.GetInt("level2iteration10", 0) == 1);
        level2iteration5noGrapple = (PlayerPrefs.GetInt("level2iteration5noGrapple", 0) == 1);
        level2iteration10noGrapple = (PlayerPrefs.GetInt("level2iteration10noGrapple", 0) == 1);

        level3iteration5 = (PlayerPrefs.GetInt("level3iteration5", 0) == 1);

        level4GetBetrayed = (PlayerPrefs.GetInt("level4GetBetrayed", 0) == 1);

        level8Get1500 = (PlayerPrefs.GetInt("level8Get1500", 0) == 1);
        level8Get2000 = (PlayerPrefs.GetInt("level8Get2000", 0) == 1);

        beatStoryMode = (PlayerPrefs.GetInt("beatStoryMode", 0) == 1);

        usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 1);
        if (usePostProcessing == 0) {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = false;
        } else {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
        }
        if (usePostProcessing == 2) {
            Camera.main.targetTexture = cameraEffect;
            defaultRawImage.SetActive(false);
            cameraEffectRawImage.SetActive(true);
            Application.targetFrameRate = 10;
        } else {
            // Camera.main.targetTexture = defaultRenderTexture;
            Camera.main.targetTexture = null;
            defaultRawImage.SetActive(true);
            cameraEffectRawImage.SetActive(false);
            Application.targetFrameRate = -1;
        }

        skipTransitions = PlayerPrefs.GetInt("skipTransitions") == 1;
        mobileSuperCheat = PlayerPrefs.GetInt("mobileSuperCheat") == 1;

        // --------------------
        // VISUAL "CHEAT CODES"
        // --------------------

        // High Detail Mode
        if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.P)) {
            Debug.Log("Enabling Post Processing");
            PlayerPrefs.SetInt("useVisualEffects", 1);
        }

        // Super Low Detail Mode
        if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.I)) {
            Debug.Log("Using Image Pixel");
            PlayerPrefs.SetInt("useVisualEffects", 2);
        }

        // Low Detail Mode
        if (Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.P)) {
            Debug.Log("Disabling Post Processing");
            PlayerPrefs.SetInt("useVisualEffects", 0);
        }

        // Show Cursor
        if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.V))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Hide Cursor
        if (Input.GetKey(KeyCode.H) && Input.GetKey(KeyCode.C))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Skip or not skip transitions
        if (Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.T))
        {
            Debug.Log("Skipping Transition Animations");
            PlayerPrefs.SetInt("skipTransitions", 1);
        }

        if (Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.T))
        {
            Debug.Log("Not skipping Transition Animations");
            PlayerPrefs.SetInt("skipTransitions", 0);
        }

        // ------------------
        // TRUE "CHEAT CODES"
        // ------------------

        // Quit to Menu
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.M)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }

        // Go to Level Select (MAIN CHEAT CODE)
        if (Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.S)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(8);
        }

        // Begin Story
        if (Input.GetKey(KeyCode.B) && Input.GetKey(KeyCode.S)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(23);
        }

        // Force Reset
        if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.R))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ------------
        // ACHIEVEMENTS
        // ------------
        if (debug) {
            unlockedHard = new bool[8] {
                true, true, true, true, true, true, true, true
            };
            unlockedEndless = new bool[8] {
                true, true, true, true, true, true, true, true
            };
        } else {
            for (int i = 1; i <= numLevels; i++) {
                beatStoryModeLevels[i - 1] = (PlayerPrefs.GetInt("beatStoryModeLevels" + i, 0) == 1);
                unlockedHard[i - 1] = (PlayerPrefs.GetInt("unlockedHard" + i, 0) == 1);
                unlockedEndless[i - 1] = (PlayerPrefs.GetInt("unlockedEndless" + i, 0) == 1);
            }

            for (int i = 0; i < numOpenExplorationCollectibles; i++) {
                openExplorationCollectibles[i] = (PlayerPrefs.GetInt("openExplorationCollectibles" + i, 0) == 1);
            }

            int newNumCollectiblesCollected = 0;
            for (int i = 0; i < numOpenExplorationCollectibles; i++) {
                newNumCollectiblesCollected += openExplorationCollectibles[i] ? 1 : 0;
            }
            numCollectiblesCollected = newNumCollectiblesCollected;
            if (numCollectiblesCollected >= 10) {
                openExplorationCollect10Collectibles = true;
            } else {
                openExplorationCollect10Collectibles = false;
            }
            if (numCollectiblesCollected >= 20) {
                openExplorationCollect20Collectibles = true;
            } else {
                openExplorationCollect20Collectibles = false;
            }

            for (int i = 0; i < numOpenExplorationLevels; i++) {
                openExplorationBallChallenges[i] = (PlayerPrefs.GetInt("openExplorationBallChallenges" + i, 0) == 1);
            }
        }
    }
}
