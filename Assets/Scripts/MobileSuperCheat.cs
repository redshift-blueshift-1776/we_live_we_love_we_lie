using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

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
        canvas = GetComponentInChildren<Canvas>();

        SceneManager.sceneLoaded += OnSceneLoaded;

        UpdateCanvasVisibility();
    }

    void Update()
    {
        mobileSuperCheat = PlayerPrefs.GetInt("mobileSuperCheat") == 1;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCanvasVisibility();
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
