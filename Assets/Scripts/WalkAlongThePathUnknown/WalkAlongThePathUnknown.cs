using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAlongThePathUnknown : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject maze;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;
    [SerializeField] public GameObject alarmAudio;

    [Header("Cameras")]
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    public bool gameActive;
    public float timer;

    public bool hitRealWall;

    public Maze_Generator mg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        alarmAudio.SetActive(false);
        cam1.SetActive(true);
        cam2.SetActive(false);
        gameActive = false;
        timer = 0f;
        hitRealWall = false;

        mg = maze.GetComponent<Maze_Generator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void soundAlarm() {
        hitRealWall = true;
        alarmAudio.SetActive(true);
        gameAudio.SetActive(false);
    }

    public IEnumerator startGame() {
        mg.VisualizeMapGeneration();
        startCanvas.SetActive(false);
        yield return new WaitForSeconds(10f);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam1.SetActive(true);
        cam2.SetActive(false);
        foreach (GameObject g in mg.removedWalls) {
            g.SetActive(true);
            Wall w = g.GetComponent<Wall>();
            w.breakable = true;
        }
        gameCanvas.SetActive(true);
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
        alarmAudio.SetActive(false);
        gameActive = true;
        timer = 0f;
        hitRealWall = false;
    }

    public void startGameButton() {
        StartCoroutine(startGame());
    }
}
