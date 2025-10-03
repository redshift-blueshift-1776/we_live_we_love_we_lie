using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class WalkAlongThePathUnknown : MonoBehaviour
{
    [SerializeField] public bool endless;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject maze;

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

        mg = maze.GetComponent<Maze_Generator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive) {
            if (timer >= 90f) {
                Fail();
            }
            timerGame.text = $"Time Remaining: {90f - Mathf.Floor(timer)}";
            timer += Time.deltaTime;
        }
    }

    public void Fail() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerPrefs.SetInt("PreviousLevel", 3);
        gameActive = false;
        // SceneManager.LoadScene(5); // Change when we have the actual scene
        transitionScript.ToFail();
    }

    public void soundAlarm() {
        hitRealWall = true;
        alarmAudio.SetActive(true);
        gameAudio.SetActive(false);
    }

    public IEnumerator startGame() {
        mg.VisualizeMapGeneration();
        startCanvas.SetActive(false);
        mazeCanvas.SetActive(true);
        float duration = 10f;
        float elapsed = 0f;
        while (elapsed < duration) {
            float t = elapsed / duration;

            timerMaze.text = $"Time to Start: {10f - Mathf.Floor(elapsed)}";

            elapsed += Time.deltaTime;
            yield return null;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam1.SetActive(true);
        cam2.SetActive(false);
        foreach (GameObject g in mg.removedWalls) {
            g.SetActive(true);
            Wall w = g.GetComponent<Wall>();
            w.breakable = true;
        }
        mazeCanvas.SetActive(false);
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

    public void wallBreak() {
        wallSoundBreak.SetActive(false);
        wallSoundBreak.SetActive(true);
    }

    public void wallFail() {
        wallSoundFail.SetActive(false);
        wallSoundFail.SetActive(true);
    }
}
