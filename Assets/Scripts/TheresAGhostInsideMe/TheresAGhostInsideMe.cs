using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class TheresAGhostInsideMe : MonoBehaviour
{
    private bool endless = false;
    private int boardsToBeat = 1;
    private float timeLimit = 120;
    [SerializeField] public GameObject titleScreenBoard;
    [SerializeField] public GameObject trap;
    [SerializeField] public GameObject key;

    [SerializeField] private GameObject easyBoards;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TMP_Text timerGame;
    [SerializeField] private TMP_Text puzzles;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;
    [SerializeField] public GameObject gameAudio2;
    [SerializeField] public GameObject failSound;
    [SerializeField] public AudioBlender ab;

    [Header("Cameras")]
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    public bool gameActive;
    public float timer;

    public bool rotateLeft;
    public bool rotateRight;
    public bool rotateUp;
    public bool rotateDown;

    private int boardsSolved;

    [SerializeField] private GameObject transition;
    [SerializeField] private Transition transitionScript;

    //gameobject containing all boards as children
    private GameObject currBoards;
    private List<GameObject> boardList = new List<GameObject>();
    private GameObject currBoard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        boardsSolved = 0;
        transitionScript = transition.GetComponent<Transition>();
        gameActive = false;
        timer = 0f;
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        winCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameAudio2.SetActive(false);
        failSound.SetActive(false);

        currBoards = easyBoards;
        foreach (Transform child in currBoards.transform)
        {
            child.gameObject.SetActive(false);
            boardList.Add(child.gameObject);
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
            if (Input.GetKeyDown(KeyCode.C)) {
                cam1.SetActive(!cam1.activeSelf);
                cam2.SetActive(!cam2.activeSelf);
            }

            if (Input.GetKey(KeyCode.W)) {
                rotateUp = true;
            } else {
                rotateUp = false;
            }
            if (Input.GetKey(KeyCode.A)) {
                rotateLeft = true;
            } else {
                rotateLeft = false;
            }
            if (Input.GetKey(KeyCode.S)) {
                rotateDown = true;
            } else {
                rotateDown = false;
            }
            if (Input.GetKey(KeyCode.D)) {
                rotateRight = true;
            } else {
                rotateRight = false;
            }
            rotateBoard();
            if (boardsSolved > boardsToBeat) {
                StartCoroutine(GameWin());
            }
            timerGame.text = $"Time Remaining: {timeLimit - Mathf.Floor(timer)}";
            ab.SetRatio(timer / timeLimit);
            puzzles.text = $"Puzzle: {Mathf.Min(boardsSolved - 1, 10)}/{boardsToBeat}";
            timer += Time.deltaTime;
        }
    }

    public IEnumerator GameWin() {
        winCanvas.SetActive(true);
        gameActive = false;
        yield return new WaitForSeconds(3f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(16);
    }

    public void GameLose() {
        failSound.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Scene currentScene = SceneManager.GetActiveScene();
        PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
        gameActive = false;
        // transitionScript.ToFail();
        StartCoroutine(LoadFailScene());
    }

    public IEnumerator LoadFailScene() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(9); // Change when we have the actual scene
    }

    public void rotateBoard() {
        Quaternion boardStart = currBoard.transform.localRotation;

        float targetXrot = 0f;
        float targetZrot = 0f;
        if (rotateUp && !rotateDown) {
            targetXrot = 10f;
        }
        if (rotateDown && !rotateUp) {
            targetXrot = -10f;
        }
        if (rotateLeft && !rotateRight) {
            targetZrot = 10f;
        }
        if (rotateRight && !rotateLeft) {
            targetZrot = -10f;
        }

        Quaternion targetRotation = Quaternion.Euler(targetXrot, 0, targetZrot);

        currBoard.transform.localRotation = Quaternion.Slerp(boardStart, targetRotation, 0.01f);
    }

    public void startGameButton() {
        startCanvas.SetActive(false);
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        cam1.SetActive(true);
        cam2.SetActive(false);
        gameCanvas.SetActive(true);
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
        gameAudio2.SetActive(true);
        gameActive = true;
        titleScreenBoard.SetActive(false);
        timer = 0f;

        nextBoard();
    }

    public void nextBoard() {
        if (boardsSolved == boardsToBeat)
        {
            return;
        }

        if (currBoard != null)
        {
            Destroy(currBoard);
        }
        currBoard = Instantiate(boardList[Random.Range(0, boardList.Count)]);
        currBoard.transform.position = new Vector3(0, 1.7f, 0);
        currBoard.SetActive(true);
        boardsSolved++;
    }
}
