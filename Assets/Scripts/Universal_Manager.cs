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
    }

    // Update is called once per frame
    void Update()
    {
        level1Layer20 = (PlayerPrefs.GetInt("level1Layer20", 0) == 1);
        level1Layer50 = (PlayerPrefs.GetInt("level1Layer50", 0) == 1);
        level1Layer100 = (PlayerPrefs.GetInt("level1Layer100", 0) == 1);

        level2iteration5 = (PlayerPrefs.GetInt("level2iteration5", 0) == 1);
        level2iteration10 = (PlayerPrefs.GetInt("level2iteration10", 0) == 1);

        level3iteration5 = (PlayerPrefs.GetInt("level3iteration5", 0) == 1);

        level4GetBetrayed = (PlayerPrefs.GetInt("level4GetBetrayed", 0) == 1);

        level8Get1500 = (PlayerPrefs.GetInt("level8Get1500", 0) == 1);
        level8Get2000 = (PlayerPrefs.GetInt("level8Get2000", 0) == 1);

        beatStoryMode = (PlayerPrefs.GetInt("beatStoryMode", 0) == 1);

        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 0);
        if (usePostProcessing == 0) {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = false;
        } else {
            UniversalAdditionalCameraData cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
        }
        if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.P)) {
            Debug.Log("Enabling Post Processing");
            PlayerPrefs.SetInt("useVisualEffects", 1);
        }
        if (Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.P)) {
            Debug.Log("Disabling Post Processing");
            PlayerPrefs.SetInt("useVisualEffects", 0);
        }
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.M)) {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.S)) {
            SceneManager.LoadScene(8);
        }
        if (Input.GetKey(KeyCode.B) && Input.GetKey(KeyCode.S)) {
            SceneManager.LoadScene(23);
        }
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
        }
    }
}
