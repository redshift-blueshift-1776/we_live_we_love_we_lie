using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class ToFindWhatIveBecome : MonoBehaviour
{
    [SerializeField] public bool endless;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] collectibles;
    [SerializeField] private List<Transform> collectibleLocations;

    [SerializeField] private List<Transform> alternateLocations;

    [SerializeField] private List<Transform> alternateLocations2;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TMP_Text timerGame;
    [SerializeField] private TMP_Text winText;
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
    [SerializeField] public float timeLimit = 169f;

    public bool[] collected = new bool[6];

    [SerializeField] private GameObject transition;
    [SerializeField] private Transition transitionScript;

    [SerializeField] private GameObject callableLyricsSyncDisplay;
    private CallableLyricsSyncDisplay clsd;

    public int usingAlternate;

    public float carSpeed = 200f;

    public int iteration;

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

        iteration = 1;

        carSpeed = 200f;

        usingAlternate = endless ? 1 : 0;
        if (endless) {
            for (int i = 0; i < 9; i++) {
                alternateLocations2.Add(collectibleLocations[collectibleLocations.Count - 1]);
                collectibleLocations.RemoveAt(collectibleLocations.Count - 1);
            }
        }
        shuffleCollectibles(usingAlternate);
    }

    public void shuffleCollectibles(int alternate) {
        Debug.Log(alternate);
        collected = new bool[6];

        if (!endless) {
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
                collectibles[i].SetActive(true);
                collectibles[i].transform.position = new Vector3(collectibleLocations[i].position.x, collectibleLocations[i].position.y, collectibleLocations[i].position.z);
            }
            return;
        }

        if (alternate == 1) {
            int n = alternateLocations.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1); // Using Unity's Random.Range
                Transform value = alternateLocations[k];
                alternateLocations[k] = alternateLocations[n];
                alternateLocations[n] = value;
            }
            for (int i = 0; i < collectibles.Length; i++) {
                collectibles[i].SetActive(true);
                collectibles[i].transform.position = new Vector3(alternateLocations[i].position.x, alternateLocations[i].position.y, alternateLocations[i].position.z);
            }
            
            // Shuffle collectibleLocations and alternateLocations2
            int clCount = collectibleLocations.Count;
            int al2Count = alternateLocations2.Count;
            collectibleLocations.AddRange(alternateLocations2);
            n = clCount + al2Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1); // Using Unity's Random.Range
                Transform value = collectibleLocations[k];
                collectibleLocations[k] = collectibleLocations[n];
                collectibleLocations[n] = value;
            }
            alternateLocations2.Clear();
            for (int i = 0; i < 9; i++) {
                alternateLocations2.Add(collectibleLocations[collectibleLocations.Count - 1]);
                collectibleLocations.RemoveAt(collectibleLocations.Count - 1);
            }
        } else if (alternate == 0) {
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
                collectibles[i].SetActive(true);
                collectibles[i].transform.position = new Vector3(collectibleLocations[i].position.x, collectibleLocations[i].position.y, collectibleLocations[i].position.z);
            }

            // Shuffle alternateLocations and alternateLocations2
            int alCount = alternateLocations.Count;
            int al2Count = alternateLocations2.Count;
            alternateLocations.AddRange(alternateLocations2);
            n = alCount + al2Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1); // Using Unity's Random.Range
                Transform value = alternateLocations[k];
                alternateLocations[k] = alternateLocations[n];
                alternateLocations[n] = value;
            }
            alternateLocations2.Clear();
            for (int i = 0; i < 9; i++) {
                alternateLocations2.Add(alternateLocations[alternateLocations.Count - 1]);
                alternateLocations.RemoveAt(alternateLocations.Count - 1);
            }
        } else {
            int n = alternateLocations2.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1); // Using Unity's Random.Range
                Transform value = alternateLocations2[k];
                alternateLocations2[k] = alternateLocations2[n];
                alternateLocations2[n] = value;
            }
            for (int i = 0; i < collectibles.Length; i++) {
                collectibles[i].SetActive(true);
                collectibles[i].transform.position = new Vector3(alternateLocations2[i].position.x, alternateLocations2[i].position.y, alternateLocations2[i].position.z);
            }
            // Shuffle collectibleLocations and alternateLocations
            int clCount = collectibleLocations.Count;
            int alCount = alternateLocations.Count;
            collectibleLocations.AddRange(alternateLocations);
            n = clCount + alCount;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1); // Using Unity's Random.Range
                Transform value = collectibleLocations[k];
                collectibleLocations[k] = collectibleLocations[n];
                collectibleLocations[n] = value;
            }
            alternateLocations.Clear();
            for (int i = 0; i < 12; i++) {
                alternateLocations.Add(collectibleLocations[collectibleLocations.Count - 1]);
                collectibleLocations.RemoveAt(collectibleLocations.Count - 1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.M)) {
            SceneManager.LoadScene(0);
        }
        if (gameActive) {
            if (timer >= timeLimit) { 
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
        if (gameActive) {
            bool win = true;
            foreach (bool b in collected) {
                win = win && b;
            }
            if (win) {
                if (endless) {
                    StartCoroutine(resetGame());
                } else {
                    StartCoroutine(GameWin());
                }
            }
        }
    }

    public IEnumerator resetGame() {
        usingAlternate = (usingAlternate + 1) % 3;
        timer = 0f;
        carSpeed = (4 * carSpeed + 100f) / 5f;
        gameActive = false;
        winCanvas.SetActive(true);
        winText.text = $"Iteration {iteration} Complete! Shuffling...";
        yield return new WaitForSeconds(3f);
        shuffleCollectibles(usingAlternate);
        gameActive = true;
        timer = 0f;
        foreach (RawImage ri in images) {
            ri.texture = blank;
        }
        iteration++;
        winCanvas.SetActive(false);
    }

    public IEnumerator GameWin() {
        winCanvas.SetActive(true);
        gameActive = false;
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject foundObject2 = GameObject.Find("Universal_Manager");
        if (foundObject2 != null) {
            Debug.Log("Found Universal_Manager");
            Universal_Manager um = foundObject2.GetComponent<Universal_Manager>();
            um.beatStoryModeLevels[1] = true;
            um.unlockedHard[1] = true;
            PlayerPrefs.SetInt("beatStoryModeLevels2", 1);
            PlayerPrefs.SetInt("unlockedHard2", 1);
        } else {
            Debug.Log("No Universal_Manager");
        }
        GameObject foundObject = GameObject.Find("StoryMode");

        // Check if the foundObject is not null
        if (foundObject != null)
        {
            Debug.Log("GameObject '" + "StoryMode" + "' found in the scene.");
            SceneManager.LoadScene(3);
        }
        else
        {
            SceneManager.LoadScene(0); // Not in story mode, goes back to the menu page
        } 
    }

    public void GameLose() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Scene currentScene = SceneManager.GetActiveScene();
        PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
        gameActive = false;
        SceneManager.LoadScene(9);
        // transitionScript.ToFail();
    }

    public void UpdateUI() {
        if (endless) {
            timerGame.text = $"Time Remaining: {timeLimit - Mathf.Floor(timer)}";
        } else {
            timerGame.text = $"Time Remaining: {timeLimit - Mathf.Floor(timer)}";
        }
        for (int i = 0; i < 6; i++) {
            images[i].texture = collected[i] ? fills[i] : blank;
        }
    }

    public void CollectItem(int itemIndex) {
        collected[itemIndex] = true;
    }
}
