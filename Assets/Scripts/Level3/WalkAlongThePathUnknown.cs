using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class WalkAlongThePathUnknown : MonoBehaviour
{
    [SerializeField] public bool hard;
    [SerializeField] public bool endless;
    [SerializeField] private GameObject player;
    [SerializeField] public GameObject goal;
    [SerializeField] public GameObject police;
    [SerializeField] private GameObject maze;
    [SerializeField] private float wallMoveTime;
    [SerializeField] private float timeLimit = 90f;
    [SerializeField] private float timeToMemorize = 10f;

    [SerializeField] public Material cutsceneMaterial;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject mazeCanvas;
    [SerializeField] private TMP_Text timerMaze;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private TMP_Text timerGame;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;
    [SerializeField] public GameObject alarmAudio;
    [SerializeField] public GameObject wallSoundBreak;
    [SerializeField] public GameObject wallSoundFail;

    [Header("Cameras")]
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    public bool gameActive;
    public float timer;

    public bool hitRealWall;

    public Maze_Generator mg;
    public Maze_Generator_Improved mg_improved;

    [SerializeField] private GameObject transition;
    [SerializeField] private Transition transitionScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transitionScript = transition.GetComponent<Transition>();
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        mazeCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        alarmAudio.SetActive(false);
        wallSoundBreak.SetActive(false);
        wallSoundFail.SetActive(false);
        cam1.SetActive(true);
        cam2.SetActive(false);
        gameActive = false;
        timer = 0f;
        hitRealWall = false;

        if (hard) {
            mg_improved = maze.GetComponent<Maze_Generator_Improved>();
            police.SetActive(false);
            // Vector2Int startCell = new Vector2Int(0, mg_improved.height - 1);
            // Vector3 startPos = police.GetComponent<Police>().GridToWorld(startCell);
            // player.transform.position = startPos;
            // police.transform.position = startPos;
        } else {
            mg = maze.GetComponent<Maze_Generator>();
        }

        if (endless) {
            goal.SetActive(true);
            timeToMemorize = 15f;
            timeLimit = 120f;
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
                Fail();
            }
            if (endless) {
                if (Vector3.Distance(player.transform.position, goal.transform.position) < 10f) {
                    StartCoroutine(startGame());
                }
            }
            timerGame.text = $"Time Remaining: {timeLimit - Mathf.Floor(timer)}";
            timer += Time.deltaTime;
        }
    }

    public void Fail() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Scene currentScene = SceneManager.GetActiveScene();
        PlayerPrefs.SetInt("PreviousLevel", currentScene.buildIndex);
        gameActive = false;
        if (hard) {
            foreach (GameObject g in mg_improved.walls) {
                if (g.activeSelf) {
                    Renderer rend = g.GetComponent<MeshRenderer>();
                    rend.material = cutsceneMaterial;
                }
            }
            foreach (GameObject g in mg_improved.removedWalls) {
                if (g.activeSelf) {
                    Renderer rend = g.GetComponent<MeshRenderer>();
                    rend.material = cutsceneMaterial;
                }
            }
        } else {
            foreach (GameObject g in mg.walls) {
                if (g.activeSelf) {
                    Renderer rend = g.GetComponent<MeshRenderer>();
                    rend.material = cutsceneMaterial;
                }
            }
            foreach (GameObject g in mg.removedWalls) {
                if (g.activeSelf) {
                    Renderer rend = g.GetComponent<MeshRenderer>();
                    rend.material = cutsceneMaterial;
                }
            }
        }
        StartCoroutine(LoadFailScene());
        // transitionScript.ToFail();
    }

    public IEnumerator LoadFailScene() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(9); // Change when we have the actual scene
    }

    public void soundAlarm() {
        hitRealWall = true;
        alarmAudio.SetActive(true);
        if (endless) {
            loadingAudio.SetActive(false);
        } else {
            gameAudio.SetActive(false);
        }
    }

    public IEnumerator startGame() {
        gameActive = false;
        alarmAudio.SetActive(false);
        if (endless) {
            loadingAudio.SetActive(true);
            // Set mg.startingSquare to the current square
            mg.GenerateGraph();
            mg.GenerateMaze();
            // Use BFS to find the furthest square and put the goal there.
        }
        if (hard) {
            mg_improved.VisualizeMapGeneration();
        } else {
            mg.VisualizeMapGeneration();
        }
        startCanvas.SetActive(false);
        mazeCanvas.SetActive(true);
        float duration = timeToMemorize;
        float elapsed = 0f;
        while (elapsed < duration) {
            float t = elapsed / duration;

            timerMaze.text = $"Time to Start: {timeToMemorize - Mathf.Floor(elapsed)}";

            elapsed += Time.deltaTime;
            yield return null;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam1.SetActive(true);
        cam2.SetActive(false);
        if (hard) {
            foreach (GameObject g in mg_improved.removedWalls) {
                g.SetActive(true);
                Wall w = g.GetComponent<Wall>();
                w.breakable = true;
            }
        } else {
            foreach (GameObject g in mg.removedWalls) {
                g.SetActive(true);
                Wall w = g.GetComponent<Wall>();
                w.breakable = true;
            }
        }
        
        mazeCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        if (!endless) {
            loadingAudio.SetActive(false);
            gameAudio.SetActive(true);
        }
        alarmAudio.SetActive(false);
        gameActive = true;
        timer = 0f;
        hitRealWall = false;
    }

    private int GridToIndex(Vector2Int cell)
    {
        if (!IsValidCell(cell))
        {
            Debug.LogError($"Invalid cell coordinates: {cell}");
            return -1; // Return invalid index
        }
        return cell.y * mazeWidth + cell.x;
    }

    private Vector2Int IndexToGrid(int index)
    {
        if (index < 0 || index >= mazeWidth * mazeHeight)
        {
            Debug.LogError($"Invalid index: {index}");
            return new Vector2Int(-1, -1); // Return invalid coordinates
        }
        return new Vector2Int(index % mazeWidth, index / mazeWidth);
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gridX = Mathf.RoundToInt((worldPos.x - topLeftX) / cellSize);
        int gridY = Mathf.RoundToInt((topLeftZ - worldPos.z) / cellSize);
        return new Vector2Int(gridX, gridY);
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        float worldX = gridPos.x * cellSize + topLeftX;
        float worldZ = topLeftZ - gridPos.y * cellSize;
        return new Vector3(worldX, 0, worldZ);
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < mazeWidth && cell.y >= 0 && cell.y < mazeHeight;
    }

    public void startGameButton() {
        StartCoroutine(startGame());
    }

    public void wallBreak() {
        wallSoundBreak.SetActive(false);
        wallSoundBreak.SetActive(true);
    }

    public void wallFail() {
        wallSoundFail.SetActive(false);
        wallSoundFail.SetActive(true);
    }
}
