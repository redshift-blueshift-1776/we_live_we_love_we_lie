using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;

public class MobileSuperCheat : MonoBehaviour
{
    public static MobileSuperCheat Instance;

    public bool mobileSuperCheat;

    public float horizontal;
    public float vertical;

    public bool jumpPressed;
    public bool interactPressed;
    public bool sprintHeld;

    public float lookX;
    public float lookY;

    private Canvas canvas;

    [SerializeField]
    private List<int> mobileScenes;

    public Dictionary<int, int> skipScenes;
    // First value is the scene that will be loaded and is unplayable
    // Second value is the scene to go to after a certain amount of time

    [SerializeField] private float autoSkipDuration = 5f;

    private Coroutine skipCoroutine;

    [SerializeField] private TMP_Text skipText;
    [SerializeField] private GameObject skipTextBackground;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist this instance
    }

    void Start()
    {
        mobileScenes = new List<int>{3, 7, 22, 24, 27, 28, 32, 36, 39, 42, 45, };
        skipScenes = new Dictionary<int, int>()
        {
            { 1, 10 },
            { 2, 35 },
            { 20, 42 }, // Level 5
            { 16, 45 }, // Level 7
            { 11, 44 }, // Level 8
            { 12, 0 }, // Level 1 hard
            { 13, 0 }, // Level 1 endless
            { 21, 0 }, // Level 2 hard
            { 14, 0 }, // Level 2 endless
            { 51, 0 }, // Level 2 obstacle course
            { 52, 0 }, // Level 2 big city
            { 19, 0 }, // Level 5 hard
            { 6, 0 }, // Level 5 superhard
            { 26, 0 }, // Level 8 hard
        };
        canvas = GetComponentInChildren<Canvas>();

        skipText.text = "Mobile Super Cheat activated.\n\nDue to budget cuts, this level has been replaced by a highly realistic simulation of success.\n\nLevel complete.";
        skipTextBackground.gameObject.SetActive(false);

        SceneManager.sceneLoaded += OnSceneLoaded;

        UpdateCanvasVisibility();
    }

    void Update()
    {
        mobileSuperCheat = PlayerPrefs.GetInt("mobileSuperCheat") == 1;
    }

    IEnumerator AutoSkipScene(int currentScene)
    {
        Debug.Log("At scene: " + currentScene + ", Going to scene: " + skipScenes[currentScene]);
        canvas.gameObject.SetActive(true);
        skipTextBackground.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(autoSkipDuration);
        Debug.Log("Going to scene: " + skipScenes[currentScene]);
        skipTextBackground.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex == currentScene)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(skipScenes[currentScene]);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCanvasVisibility();

        if (!mobileSuperCheat)
        {
            return;
        }
            

        if (skipScenes.ContainsKey(scene.buildIndex))
        {
            if (skipCoroutine != null)
            {
                StopCoroutine(skipCoroutine);
            }

            skipCoroutine = StartCoroutine(AutoSkipScene(scene.buildIndex));
        }
    }

    void UpdateCanvasVisibility()
    {
        bool shouldShow =
            mobileSuperCheat &&
            mobileScenes.Contains(SceneManager.GetActiveScene().buildIndex);

        canvas.gameObject.SetActive(shouldShow);
    }

    void LateUpdate()
    {
        // ResetFrameInputs();

        lookX = 0;
        lookY = 0;
    }

    // public void ResetFrameInputs()
    // {
    //     jumpPressed = false;
    //     interactPressed = false;
    // }
}
