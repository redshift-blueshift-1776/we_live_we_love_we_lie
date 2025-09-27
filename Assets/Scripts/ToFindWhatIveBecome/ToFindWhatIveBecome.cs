using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class ToFindWhatIveBecome : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] collectibles;
    [SerializeField] private List<Transform> collectibleLocations;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TMP_Text timerGame;
    [SerializeField] private RawImage[] images;
    [SerializeField] private Texture[] fills;
    [SerializeField] private Texture blank;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    [Header("Cameras")]
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    public bool gameActive;
    public float timer;

    public bool[] collected = new bool[6];

    [SerializeField] private GameObject transition;
    [SerializeField] private Transition transitionScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transitionScript = transition.GetComponent<Transition>();
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        winCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        cam2.SetActive(true);
        cam1.SetActive(false);
        gameActive = false;
        timer = 0f;

        int n = collectibleLocations.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // Using Unity's Random.Range
            Transform value = collectibleLocations[k];
            collectibleLocations[k] = collectibleLocations[n];
            collectibleLocations[n] = value;
        }
        for (int i = 0; i < collectibles.Length; i++) {
            collectibles[i].transform.position = new Vector3(collectibleLocations[i].position.x, collectibleLocations[i].position.y, collectibleLocations[i].position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive) {
            if (timer >= 292f) { 
                GameLose(); // Change when we have the actual scene
            }
            UpdateUI();
            CheckEndConditions();
            timer += Time.deltaTime;
        }
    }

    public void startGameButton() {
        startCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam1.SetActive(true);
        cam2.SetActive(false);
        gameCanvas.SetActive(true);
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
        gameActive = true;
        timer = 0f;
        foreach (RawImage ri in images) {
            ri.texture = blank;
        }
    }

    public void CheckEndConditions() {
        bool win = true;
        foreach (bool b in collected) {
            win = win && b;
        }
        if (win) {
            StartCoroutine(GameWin());
        }
    }

    public IEnumerator GameWin() {
        winCanvas.SetActive(true);
        gameActive = false;
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(3);
    }

    public void GameLose() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerPrefs.SetInt("PreviousLevel", 2);
        gameActive = false;
        transitionScript.ToFail();
    }

    public void UpdateUI() {
        timerGame.text = $"Time Remaining: {292f - Mathf.Floor(timer)}";
        for (int i = 0; i < 6; i++) {
            images[i].texture = collected[i] ? fills[i] : blank;
        }
    }

    public void CollectItem(int itemIndex) {
        collected[itemIndex] = true;
    }
}
