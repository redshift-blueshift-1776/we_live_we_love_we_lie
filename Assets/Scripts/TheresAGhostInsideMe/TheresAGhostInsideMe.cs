using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class TheresAGhostInsideMe : MonoBehaviour
{
    [SerializeField] public GameObject board;
    [SerializeField] public GameObject trap;
    [SerializeField] public GameObject key;

    [SerializeField] public GameObject[] boards;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private TMP_Text timerGame;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    [Header("Cameras")]
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

    public bool gameActive;
    public float timer;

    public bool rotateLeft;
    public bool rotateRight;
    public bool rotateUp;
    public bool rotateDown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameActive = false;
        timer = 0f;
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        winCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        board = boards[0];
        board.SetActive(true);
        for (int i = 1; i < boards.Length; i++) {
            boards[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive) {
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
        }
    }

    public void rotateBoard() {
        Quaternion boardStart = board.transform.localRotation;

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

        board.transform.localRotation = Quaternion.Slerp(boardStart, targetRotation, 0.01f);
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
        gameActive = true;
        timer = 0f;
    }

    public void nextBoard() {
        GameObject newBoard = boards[0];
    }
}
