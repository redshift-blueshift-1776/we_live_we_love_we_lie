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
    [SerializeField] private TMP_Text scoreGame;

    [Header("Audio")]
    [SerializeField] public GameObject loadingAudio;
    [SerializeField] public GameObject gameAudio;

    public bool gameActive;

    public float timer;

    public List<GameObject> notes;

    public int score;

    private double secondsPerBeat;

    private double nextChangeTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondsPerBeat = 60.0 / 145.0 / 4.0;
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
            scoreGame.text = "Score: " + score;
            timer += Time.deltaTime;
        }
    }

    public void GenerateNotes() {
        if (hard) {
            string[] notes = {
            "16,1,0,10",
            "20,1,0,10",
            "24,1,0,0",
            "28,1,0,-10",
            "32,1,0,-20",
            };
        } else {
            string[] notes = {
            "64,10,0,20",
            "80,5,0,20",
            "-96,0,0,20",
            "112,-5,0,20",
            "128,-10,0,20",
            "144,-10,5,20",
            "152,-5,5,20",
            "-160,0,5,20",
            "168,5,5,20",
            "176,10,5,20",
            "256,10,0,120",
            "272,5,0,120",
            "288,0,0,120",
            "296,0,5,120",
            "304,-5,5,120",
            "320,-10,0,120",
            };

            foreach (string n in notes) {
                string[] parts = n.Split(',');
                if (parts.Length < 4) continue;

                if (float.TryParse(parts[0], out float duration)) {
                    float.TryParse(parts[1], out float x_pos);
                    float.TryParse(parts[2], out float y_pos);
                    float.TryParse(parts[3], out float z_pos);
                    GameObject newNote = Instantiate(note);
                    newNote.transform.localPosition = player.transform.localPosition + new Vector3(x_pos, y_pos, z_pos);
                    Note newNoteScript = newNote.GetComponent<Note>();
                    newNoteScript.gm = gameObject.GetComponent<SecondWeLiveWeLoveWeLie>();
                    newNoteScript.duration = 16f * (float) secondsPerBeat;
                    newNoteScript.delay = Mathf.Abs(duration * (float) secondsPerBeat) - 8f * (float) secondsPerBeat;
                    newNoteScript.realNote = (duration > 0);
                }
            }
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

    public void addScore(int scoreToAdd) {
        score += scoreToAdd;
    }

    public IEnumerator LoadFailScene() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(9); // Change when we have the actual scene
    }
}
