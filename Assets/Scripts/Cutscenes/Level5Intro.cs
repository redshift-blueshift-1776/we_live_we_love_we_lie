using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Level5Intro : MonoBehaviour
{
    [Header("Cutscene Objects")]
    [SerializeField] private GameObject cameras;
    [SerializeField] public GameObject transition;

    [Header("Cutscene Text")]

    public List<String> lyrics;

    [SerializeField] private TMP_Text lyricsDisplay;
    [SerializeField] private GameObject lyricsBackground;
    [SerializeField] private float beatsPerMinute = 900f;
    [SerializeField] private GameObject canvas;

    private int currentLine = 0;
    private double nextLyricTime;
    private float secondsPerBeat;
    private Coroutine doLyricsCoroutine;
    private Coroutine fillLyricsCoroutine;
    private Coroutine lineFillCoroutine;
    private bool isAutoPlaying = true;

    [SerializeField] private RectTransform lyricsProgressBarFill;
    private float originalLyricsBarWidth;

    [SerializeField] private RectTransform lyricsLineFillBar;
    [SerializeField] private bool autoStart;

    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float minLineDuration = 2.0f;
    [SerializeField] private float holdDuration = 0.5f;

    [SerializeField] public int nextSceneIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in cameras.transform)
        {
            cameraList.Add(child.gameObject);
        }
        Debug.Log(cameraList);
        Debug.Log(cameraList.Count);
        activateCamera(0);

        lyrics = new List<string> {
            "Welcome to the FIFTH challenge made by Lan Atlas!",
            "This is not the same as the ones before!",
            "",
            "Your fitness skills will be put to the test...",
            "You will have to jump from platform to platform...",
            "There are ladders to climb!",  //5
            "Ice to slip on!",
            "Slime to bounce on!",
            "Crumbling platforms too!",
            "I spent a lot of money for these...",
            "Finally, we have cassette blocks that alternate in and out!",
            "",
            "Reach the heart at the end to pass!",
            "Good luck!"
        };
        secondsPerBeat = 60f / beatsPerMinute;
        // lyrics = lyricsText;

        nextLyricTime = 0;
        currentLine = 0;
        lyricsDisplay.text = "";
        lyricsBackground.SetActive(true);
        canvas.SetActive(false);
        originalLyricsBarWidth = lyricsProgressBarFill.sizeDelta.x;
        showLyrics();
    }

    List<GameObject> cameraList = new List<GameObject>();
    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (currentLine == 4)
        {
            activateCamera(1);
        }
        else if (currentLine == 5)
        {
            activateCamera(2);
        }
        else if (currentLine == 6)
        {
            activateCamera(3);
        }
        else if (currentLine == 7)
        {
            activateCamera(4);
        }
        else if (currentLine == 8)
        {
            activateCamera(5);
        }
        else if (currentLine == 10)
        {
            activateCamera(6);
        } else if (currentLine == 12)
        {
            activateCamera(7);
        }
        //if (currentLine == 8)
        //{
        //    cam1.SetActive(true);
        //    cam2.SetActive(false);
        //}
    }

    private void activateCamera(int index)
    {
        for (int i = 0; i < cameraList.Count; i++)
        {
            cameraList[i].SetActive(false);
        }
        cameraList[index].SetActive(true);
    }
    public void OnSkipPressed()
    {
        StopAutoPlay();
        ClearLyrics();
    }

    public void StopAutoPlay()
    {
        isAutoPlaying = false;

        if (doLyricsCoroutine != null) StopCoroutine(doLyricsCoroutine);
        if (fillLyricsCoroutine != null) StopCoroutine(fillLyricsCoroutine);
        if (lineFillCoroutine != null) StopCoroutine(lineFillCoroutine);

        doLyricsCoroutine = fillLyricsCoroutine = lineFillCoroutine = null;
        lyricsLineFillBar.sizeDelta = new Vector2(0f, lyricsLineFillBar.sizeDelta.y);
    }

    public void showLyrics()
    {
        nextLyricTime = AudioSettings.dspTime;
        currentLine = 0;
        lyricsDisplay.text = "";
        canvas.SetActive(true);
        isAutoPlaying = true;

        ResetLyricsProgressBar();

        if (doLyricsCoroutine != null)
            StopCoroutine(doLyricsCoroutine);
        doLyricsCoroutine = StartCoroutine(doLyrics());
    }

    private IEnumerator doLyrics()
    {
        nextLyricTime = AudioSettings.dspTime;
        currentLine = 0;

        while (currentLine < lyrics.Count && isAutoPlaying)
        {
            yield return fillLyrics(lyrics[currentLine]);
            currentLine++;
        }

        // Fade everything out and move to next scene
        lyricsDisplay.text = "";
        canvas.SetActive(false);

        if (nextSceneIndex >= 0)
            SceneManager.LoadScene("Level 5 Supernerfed Joke");

        doLyricsCoroutine = null;
    }

    private void UpdateLyricsDisplay()
    {
        if (currentLine < 0 || currentLine >= lyrics.Count) return;

        string line = lyrics[currentLine];
        if (fillLyricsCoroutine != null) StopCoroutine(fillLyricsCoroutine);
        fillLyricsCoroutine = StartCoroutine(fillLyrics(line));

        // UpdateLyricsProgressBar(startLineFill: true);
    }

    private IEnumerator fillLyrics(string line)
    {
        // Progess bar
        // float segmentWidth = originalLyricsBarWidth / (lyrics.Count - 1);
        float segmentWidth = originalLyricsBarWidth / (lyrics.Count);
        float offsetX = segmentWidth * currentLine;
        float duration = Mathf.Max(minLineDuration, line.Length * secondsPerBeat);
        UpdateLyricsProgressBar(segmentWidth, offsetX, duration + fadeInDuration + fadeOutDuration);

        TMP_Text target = lyricsDisplay;

        // bad typing animtions
        float elapsed = 0f;
        target.text = "";
        target.alpha = 1f;
        while (elapsed < duration)
        {
            int charsShown = Mathf.Clamp((int)(line.Length * (elapsed / duration)), 0, line.Length);
            target.text = line.Substring(0, charsShown);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.text = line;

        // Hold
        yield return new WaitForSecondsRealtime(holdDuration);

        // Fade
        float ft = 0f;
        while (ft < fadeOutDuration)
        {
            float a = 1f - (ft / fadeOutDuration);
            target.alpha = a;
            ft += Time.deltaTime;
            yield return null;
        }

        target.alpha = 0f;

        target.text = "";

        // Advance the total progress bar (accumulated fill)
        lyricsProgressBarFill.sizeDelta = new Vector2(offsetX + segmentWidth, lyricsProgressBarFill.sizeDelta.y);
    }


    private void UpdateLyricsProgressBar(float segmentWidth, float offsetX, float duration)
    {
        if (lineFillCoroutine != null)
            StopCoroutine(lineFillCoroutine);

        lineFillCoroutine = StartCoroutine(FillLineBar(duration, segmentWidth, offsetX));
    }

    private IEnumerator FillLineBar(float duration, float segmentWidth, float offsetX)
    {
        // lyricsProgressBarFill.sizeDelta = new Vector2(offsetX, lyricsProgressBarFill.sizeDelta.y);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            lyricsLineFillBar.sizeDelta = new Vector2(segmentWidth * t, lyricsLineFillBar.sizeDelta.y);
            lyricsLineFillBar.anchoredPosition = new Vector2(offsetX, lyricsLineFillBar.anchoredPosition.y);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        lyricsLineFillBar.sizeDelta = new Vector2(segmentWidth, lyricsLineFillBar.sizeDelta.y);
    }

    private void ResetLyricsProgressBar()
    {
        lyricsProgressBarFill.sizeDelta = new Vector2(0, lyricsProgressBarFill.sizeDelta.y);
    }

    private void ClearLyrics()
    {
        lyricsDisplay.text = "";
        lyricsBackground.SetActive(false);
        canvas.SetActive(false);
        SceneManager.LoadScene("Level 5 Supernerfed Joke");
    }
}
