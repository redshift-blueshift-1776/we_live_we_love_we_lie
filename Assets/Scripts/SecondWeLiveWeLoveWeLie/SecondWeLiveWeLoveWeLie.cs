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

            "256,110,0,120",
            "272,105,0,120",
            "288,100,0,120",
            "296,100,5,120",
            "304,95,5,120",

            // Drop
            "320,90,0,150",
            "324,90,5,150",
            "326,90,10,150",
            "328,95,10,150",
            "330,100,10,150",
            "332,105,10,150",

            "336,110,10,150",
            "339,110,14,150",
            "340,110,15,150",
            "342,105,15,150",
            "344,100,15,150",
            "346,95,15,150",
            "348,90,15,150",

            "352,90,20,150",
            "354,95,20,150",
            "356,100,20,150",
            "358,105,20,150",
            "360,110,20,150",
            "362,110,25,150",
            "364,105,25,150",

            "368,100,25,150",
            "370,95,30,150",
            "372,100,30,150",
            "374,105,30,150",
            "376,110,30,150",
            "378,110,35,150",
            "380,105,35,150",
            "382,100,35,150",

            "384,100,40,150",
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

            "256,110,0,120",
            "272,105,0,120",
            "288,100,0,120",
            "296,100,5,120",
            "304,95,5,120",

            // Drop
            "320,90,0,120",
            "324,90,5,120",
            // "-326,90,10,120",
            "-328,95,10,120",
            // "-330,100,10,150",
            "332,105,10,120",

            "336,110,10,120",
            // "-339,110,14,150",
            // "-340,110,15,120",
            // "-342,105,15,150",
            "344,100,15,120",
            // "-346,95,15,150",
            "-348,90,15,120",

            "352,90,20,120",
            // "354,95,20,150",
            // "356,100,20,120",
            // "358,105,20,150",
            "-360,110,20,120",
            // "362,110,25,150",
            "364,105,25,120",

            "368,100,25,120",
            "-370,95,30,120",
            "-372,100,30,120",
            "374,105,30,120",
            // "376,110,30,120",
            // "378,110,35,150",
            "380,105,35,120",
            // "382,100,35,150",

            "384,100,40,120",
            // "388,95,40,120",
            // "390,90,40,120",
            "392,90,45,120",
            // "396,96,45,120",
            // "397,97,45,120",
            // "398,98,45,120",
            // "399,99,45,120",
            // "400,100,45,120",
            "404,105,45,120",
            // "406,110,45,120",
            // "408,110,50,120",

            "416,100,50,120",
            "420,95,50,120",
            // "422,90,50,120",
            // "424,90,55,120",
            "428,96,55,120",
            "429,97,55,120",
            "430,98,55,120",
            "431,99,55,120",
            "432,100,55,120",
            // "436,105,55,120",
            // "438,110,55,120",
            // "440,110,60,120",

            "448,100,60,120",
            };

            // string[] notes = {
            // "64,10,0,20",
            // "80,5,0,20",
            // "-96,0,0,20",
            // "112,-5,0,20",
            // "128,-10,0,20",
            // "144,-10,5,20",
            // "152,-5,5,20",
            // "-160,0,5,20",
            // "168,5,5,20",
            // "176,10,5,20",

            // "256,110,0,120",
            // "272,105,0,120",
            // "288,100,0,120",
            // "296,100,5,120",
            // "304,95,5,120",

            // // Drop
            // "320,90,0,120",
            // "324,90,5,120",
            // "326,90,10,120",
            // "328,95,10,120",
            // "330,100,10,120",
            // "332,105,10,120",

            // "336,110,10,120",
            // "339,110,14,120",
            // "340,110,15,120",
            // "342,105,15,120",
            // "344,100,15,120",
            // "346,95,15,120",
            // "348,90,15,120",

            // "352,90,20,120",
            // "354,95,20,120",
            // "356,100,20,120",
            // "358,105,20,120",
            // "360,110,20,120",
            // "362,110,25,120",
            // "364,105,25,120",

            // "368,100,25,120",
            // "370,95,30,120",
            // "372,100,30,120",
            // "374,105,30,120",
            // "376,110,30,120",
            // "378,110,35,120",
            // "380,105,35,120",
            // "382,100,35,120",

            // "384,100,40,120",
            // "388,95,40,120",
            // "390,90,40,120",
            // "392,90,45,120",
            // "396,96,45,120",
            // "397,97,45,120",
            // "398,98,45,120",
            // "399,99,45,120",
            // "400,100,45,120",
            // "404,105,45,120",
            // "406,110,45,120",
            // "408,110,50,120",

            // "416,100,50,120",
            // "420,95,50,120",
            // "422,90,50,120",
            // "424,90,55,120",
            // "428,96,55,120",
            // "429,97,55,120",
            // "430,98,55,120",
            // "431,99,55,120",
            // "432,100,55,120",
            // "436,105,55,120",
            // "438,110,55,120",
            // "440,110,60,120",

            // "448,100,60,120",
            // };

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
                    // newNoteScript.delay = Mathf.Abs(duration * (float) secondsPerBeat);
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
