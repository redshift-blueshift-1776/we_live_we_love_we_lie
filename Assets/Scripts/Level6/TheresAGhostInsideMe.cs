using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheresAGhostInsideMe : MonoBehaviour
{
    [SerializeField] private GameObject titleScreenBoard;

    [Header("Boards")]
    [SerializeField] private GameObject easyBoards;
    [SerializeField] private GameObject hardBoards;
    [SerializeField] private GameObject testBoards;
    

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TMP_Text timerGame;
    [SerializeField] private TMP_Text puzzles;

    [Header("Audio")]
    [SerializeField] private GameObject loadingAudio;
    [SerializeField] private GameObject gameAudio;
    [SerializeField] private GameObject gameAudio2;
    [SerializeField] private GameObject failSound;
    [SerializeField] private AudioBlender ab;

    [Header("Cameras")]
    [SerializeField] private GameObject cam1;
    [SerializeField] private GameObject cam2;

    private bool gameActive;
    private float timer;

    private bool rotateLeft;
    private bool rotateRight;
    private bool rotateUp;
    private bool rotateDown;

    private int boardsSolved;

    [SerializeField] private GameObject transition;
    [SerializeField] private Transition transitionScript;

    //gameobject containing all boards as children
    private GameObject currBoards;
    private List<GameObject> boardList = new List<GameObject>();
    private GameObject currBoard;


    private bool endless = false;
    private int boardsToBeat = 10;
    private float timeLimit = 120;
    private int currKeysLeft = 1000;

    private int difficulty = 1;

    public bool testingMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setDifficulty();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;
        boardsSolved = -1;
        transitionScript = transition.GetComponent<Transition>();
        gameActive = false;
        timer = 0f;
        initializeUI();
        initializeAudio();

        //change based on difficulty
        switch (difficulty)
        {
            case 0:
                currBoards = easyBoards;
                break;
            case 1:
                currBoards = hardBoards;
                break;
            default:
                currBoards = easyBoards;
                break;
        }
        foreach (Transform child in currBoards.transform)
        {
            boardList.Add(child.gameObject);
        }
    }

    private void setDifficulty()
    {
        GameObject difficultyInfo = GameObject.FindGameObjectWithTag("DifficultyInfo");
        if (difficultyInfo != null)
        {
            difficulty = (int)char.GetNumericValue(difficultyInfo.name[difficultyInfo.name.Length - 1]);
        }
    }

    private void initializeUI()
    {
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        winCanvas.SetActive(false);
    }

    private void initializeAudio()
    {
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameAudio2.SetActive(false);
        failSound.SetActive(false);
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
            if (boardsSolved == boardsToBeat) {
                StartCoroutine(GameWin());
            }
            timerGame.text = $"Time Remaining: {timeLimit - Mathf.Floor(timer)}";
            ab.SetRatio(timer / timeLimit);
            puzzles.text = $"Puzzle: {Mathf.Min(boardsSolved, 10)}/{boardsToBeat}";
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

    public void collectKey()
    {
        currKeysLeft--;
        if (currKeysLeft == 0)
        {
            nextBoard();
        }
    }

    private void nextBoard() {
        boardsSolved++;
        if (!gameActive || boardsSolved == boardsToBeat)
        {
            return;
        }

        if (currBoard != null)
        {
            Destroy(currBoard);
        }
        GameObject originalBoard = boardList[testingMode ? 0 : Random.Range(0, boardList.Count)];

        currBoard = Instantiate(originalBoard);
        currKeysLeft = 0;
        foreach (Transform child in currBoard.transform)
        {
            if (child.CompareTag("Level6Key"))
            {
                currKeysLeft++;
            }
        }
        currBoard.transform.position = new Vector3(0, 1.7f, 0);
        currBoard.SetActive(true);
        

    }
}
