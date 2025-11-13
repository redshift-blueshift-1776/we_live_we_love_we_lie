using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using System.Linq;
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
    [SerializeField] public float policeSpeed = 10f;

    [Header("Maze Configuration")]
    [SerializeField] public GameObject mazeGeneratorObject;
    [SerializeField] public Maze_Generator mazeGenerator;  // Reference to the maze
    [SerializeField] public int mazeWidth = 5;  // X-axis size
    [SerializeField] public int mazeHeight = 5; // Y-axis size

    [Header("Grid Settings")]
    [SerializeField] public float cellSize = 50f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftX = -100f;  // Distance between grid points in Unity world units
    [SerializeField] public float topLeftZ = 100f;  // Distance between grid points in Unity world units

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

    public int iteration;

    public bool hitRealWall;

    public Maze_Generator mg;
    public Maze_Generator_Improved mg_improved;

    [SerializeField] private GameObject transition;
    [SerializeField] private Transition transitionScript;

    public Vector2Int startingSquare;
    public Vector2Int goalSquare;
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
        iteration = 1;
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
                    iteration++;
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

    private Vector2Int FindFurthestCellFromStart()
    {
        var edges = mg.getMST();
        Dictionary<int, List<int>> adj = new Dictionary<int, List<int>>();
        foreach (var (from, to) in edges)
        {
            if (!adj.ContainsKey(from)) adj[from] = new List<int>();
            if (!adj.ContainsKey(to)) adj[to] = new List<int>();
            adj[from].Add(to);
            adj[to].Add(from);
        }

        Queue<int> queue = new Queue<int>();
        HashSet<int> visited = new HashSet<int>();
        queue.Enqueue(GridToIndex(startingSquare));
        visited.Add(GridToIndex(startingSquare));
        int last = GridToIndex(startingSquare);

        while (queue.Count > 0)
        {
            last = queue.Dequeue();
            foreach (int neighbor in adj[last])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return IndexToGrid(last);
    }

    public IEnumerator startGame() {
        gameActive = false;
        gameCanvas.SetActive(false);
        alarmAudio.SetActive(false);
        loadingAudio.SetActive(true);
        if (endless)
        {
            timeToMemorize = Mathf.Max(3f, timeToMemorize * 0.9f); 
        }

        if (endless) {
            // Visual transition effect
            StartCoroutine(EndlessTransitionEffect());
            yield return new WaitForSeconds(1f);
            // Find the furthest cell from the current start
            if (iteration == 1) {
                mg.GenerateGraph();
                mg.GenerateMaze();
                goalSquare = new Vector2Int(4,4);
            } else {
                // Start from previous goal or origin
                startingSquare = goalSquare;  
                mg.startingSquare = GridToIndex(startingSquare);
                mg.GenerateGraph();
                mg.GenerateMaze();

                // Find the new goal
                goalSquare = FindFurthestCellFromStart();

                // Move the goal GameObject in world space
                goal.transform.position = GridToWorld(goalSquare);
            }
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
            if (endless) {
                timerMaze.text = $"Time to Start: {Mathf.Floor(10f * (timeToMemorize - elapsed)) / 10f}";
            } else {
                timerMaze.text = $"Time to Start: {timeToMemorize - Mathf.Floor(elapsed)}";
            }

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

    private IEnumerator EndlessTransitionEffect()
    {
        Color newColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        foreach (GameObject g in mg.walls.Concat(mg.removedWalls).ToList())
        {
            g.SetActive(true);
            Renderer rend = g.GetComponent<MeshRenderer>();
            rend.material.color = newColor;
        }
        yield return null;
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
