using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SecondWeLiveWeLoveWeLie : MonoBehaviour
{
    [SerializeField] public GameObject player;

    [SerializeField] public GameObject note;

    [SerializeField] public bool hard;

    [Header("Canvasses")]
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private TMP_Text timerGame;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    public bool gameActive;

    public float timer;

    public List<GameObject> notes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loadingAudio.SetActive(true);
        gameAudio.SetActive(false);
        gameActive = false;
        timer = 0f;

        GenerateNotes();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive) {
            if (timer >= 169f) { 
                Fail(); // Change when we have the actual scene
            }
            timer += Time.deltaTime;
        }
    }

    public void GenerateNotes() {
        if (hard) {

        }
    }

    public void startGameButton() {
        startCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameCanvas.SetActive(true);
        loadingAudio.SetActive(false);
        gameAudio.SetActive(true);
        gameActive = true;
        timer = 0f;
    }

    public void Fail() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerPrefs.SetInt("PreviousLevel", 11);
        gameActive = false;
        StartCoroutine(LoadFailScene());
        // transitionScript.ToFail();
    }

    public IEnumerator LoadFailScene() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(9); // Change when we have the actual scene
    }
}
