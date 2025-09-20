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

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private TMP_Text timerGame;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    [Header("Cameras")]
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    public bool gameActive;
    public float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        cam2.SetActive(true);
        cam1.SetActive(false);
        gameActive = false;
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive) {
            if (timer >= 292f) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PlayerPrefs.SetInt("PreviousLevel", 2);
                SceneManager.LoadScene(5); // Change when we have the actual scene
            }
            timerGame.text = $"Time Remaining: {292f - Mathf.Floor(timer)}";
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
    }
}
