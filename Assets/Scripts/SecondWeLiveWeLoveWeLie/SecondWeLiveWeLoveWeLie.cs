using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

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

    public string[] SonicBlasterPattern(int time_start, int x_start, int y_start, int z_start) {
        string[] ret = new string[18];
        if (hard) {
            ret = new string[18];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 5;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 1;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 5;
            ret[12] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[13] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[14] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[15] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 5;
            ret[16] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 5;
            ret[17] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[12];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            time_start += 2;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            y_start += 5;
            time_start += 2;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            time_start += 2;
            y_start += 1;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            time_start += 2;
            y_start += 5;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[9] = "" + (time_start * -1) + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            time_start += 2;
            y_start += 5;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
    }

    public string[] SpectrePattern(int time_start, int x_start, int y_start, int z_start) {
        string[] ret = new string[12];
        if (hard) {
            ret = new string[12];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 5;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[2] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            ret[3] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[4] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[5] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[6] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[7] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 5;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            ret[10] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            ret[11] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        } else {
            ret = new string[10];
            ret[0] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 5;
            ret[1] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start -= 5;
            time_start += 2;
            ret[2] = "" + (time_start * -1) + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            y_start += 1;
            ret[3] = "" + (time_start * -1) + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[4] = "" + (time_start * -1) + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[5] = "" + (time_start * -1) + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[6] = "" + (time_start * -1) + "," + x_start + "," + y_start + "," + z_start;
            time_start += 1;
            y_start += 1;
            ret[7] = "" + (time_start * -1) + "," + x_start + "," + y_start + "," + z_start;
            time_start += 4;
            x_start += 5;
            ret[8] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
            time_start += 2;
            x_start += 5;
            time_start += 2;
            ret[9] = "" + time_start + "," + x_start + "," + y_start + "," + z_start;
        }
        return ret;
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
            "320,90,0,120",
            "324,90,5,120",
            "326,90,10,120",
            "328,95,10,120",
            "330,100,10,120",
            "332,105,10,120",

            "336,110,10,120",
            "338,110,13,120",
            "339,110,14,120",
            "340,110,15,120",
            "342,105,15,120",
            "344,100,15,120",
            "346,95,15,120",
            "348,90,15,120",

            "352,90,20,120",
            "354,95,20,120",
            "356,100,20,120",
            "358,105,20,120",
            "360,110,20,120",
            "362,110,25,120",
            "364,105,25,120",

            "368,100,25,120",
            "370,95,30,120",
            "372,100,30,120",
            "374,105,30,120",
            "376,110,30,120",
            "378,110,35,120",
            "380,105,35,120",
            "382,100,35,120",

            "384,100,40,120",
            "388,95,40,120",
            "390,90,40,120",
            "392,90,45,120",
            "396,96,45,120",
            "397,97,45,120",
            "398,98,45,120",
            "399,99,45,120",
            "400,100,45,120",
            "404,105,45,120",
            "406,110,45,120",
            "408,110,50,120",

            "416,100,50,120",
            "420,95,50,120",
            "422,90,50,120",
            "424,90,55,120",
            "428,96,55,120",
            "429,97,55,120",
            "430,98,55,120",
            "431,99,55,120",
            "432,100,55,120",
            "436,105,55,120",
            "438,110,55,120",
            "440,110,60,120",

            "448,100,60,120",
            "450,95,60,120",
            "452,90,60,120",
            "454,90,65,120",
            "456,95,65,120",
            "458,100,65,120",
            "460,100,66,120",
            "461,100,67,120",
            "462,100,68,120",
            "463,100,69,120",
            "464,100,70,120",
            "466,105,70,120",
            "468,110,70,120",
            "470,110,75,120",
            "472,105,75,120",
            "474,100,75,120",
            "476,100,80,120",
            "478,100,85,120",
            };

            notes = notes.Concat(SonicBlasterPattern(480, 95, 85, 120)).ToArray();

            notes = notes.Concat(SpectrePattern(512, 70, 95, 120)).ToArray();

            notes = notes.Concat(SpectrePattern(544, 45, 90, 120)).ToArray();

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
            // "-328,95,5,120",
            "332,90,10,120",

            "336,100,10,120",
            "344,100,15,120",
            "-348,95,15,120",

            "352,95,20,120",
            "-360,100,20,120",
            "364,100,25,120",

            "368,100,30,120",
            "-370,90,30,120",
            "380,100,35,120",

            "384,100,40,120",
            "392,90,45,120",
            "-400,95,45,120",
            "404,100,45,120",

            "416,100,50,120",
            "420,95,50,120",
            "428,96,55,120",
            "429,97,55,120",
            "430,98,55,120",
            "431,99,55,120",
            "432,100,55,120",

            "448,100,60,120",
            // "450,95,60,120",
            // "452,90,60,120",
            // "454,90,65,120",
            "456,95,65,120",
            // "458,100,65,120",
            "460,100,66,120",
            "461,100,67,120",
            "462,100,68,120",
            "463,100,69,120",
            "464,100,70,120",
            // "466,105,70,120",
            // "468,110,70,120",
            // "470,110,75,120",
            "472,105,75,120",
            // "474,100,75,120",
            // "476,100,80,120",
            // "478,100,85,120",
            };

            notes = notes.Concat(SonicBlasterPattern(480, 95, 85, 120)).ToArray();

            notes = notes.Concat(SpectrePattern(512, 70, 95, 120)).ToArray();

            notes = notes.Concat(SpectrePattern(544, 45, 90, 120)).ToArray();

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
            // "450,95,60,120",
            // "452,90,60,120",
            // "454,90,65,120",
            // "456,95,65,120",
            // "458,100,65,120",
            // "460,100,66,120",
            // "461,100,67,120",
            // "462,100,68,120",
            // "463,100,69,120",
            // "464,100,70,120",
            // "466,105,70,120",
            // "468,110,70,120",
            // "470,110,75,120",
            // "472,105,75,120",
            // "474,100,75,120",
            // "476,100,80,120",
            // "478,100,85,120",
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
