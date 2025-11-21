using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Level6Intro : MonoBehaviour
{
    [Header("Cutscene Objects")]
    [SerializeField] public GameObject cam1;
    [SerializeField] public GameObject cam2;

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

    [SerializeField] public int nextSceneIndex = 3;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);

        lyrics = new List<string> {
            "Welcome to the SIXTH challenge!",
            "The last challenge was pretty long and difficult...",
            "And while this challenge is much shorter, it is still very difficult.",
            "As a game developer, it is important to know what parts need modifying and which parts to leave put...",
            "This challenge will test that...",
            "In this challenge, you will have to solve ten puzzles in two minutes.",
            "Each puzzle has two blocks inside, one red trap and one yellow key.",
            "You must tilt the board on the table to get the yellow key out without letting the red trap escape.",
            "Many of these will require you to use the walls inside of the puzzle or use some clever tricks.",
            "Good luck, contestants. There are only a few challenges left."
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

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (currentLine == 5) {
            cam1.SetActive(false);
            cam2.SetActive(true);
        }
        if (currentLine == 9) {
            cam1.SetActive(true);
            cam2.SetActive(false);
        }
    }

    public void OnSkipPressed() {
        StopAutoPlay();
        ClearLyrics();
    }

    public void StopAutoPlay() {
        isAutoPlaying = false;

        if (doLyricsCoroutine != null) StopCoroutine(doLyricsCoroutine);
        if (fillLyricsCoroutine != null) StopCoroutine(fillLyricsCoroutine);
        if (lineFillCoroutine != null) StopCoroutine(lineFillCoroutine);

        doLyricsCoroutine = fillLyricsCoroutine = lineFillCoroutine = null;
        lyricsLineFillBar.sizeDelta = new Vector2(0f, lyricsLineFillBar.sizeDelta.y);
    }

    public void showLyrics() {
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
            SceneManager.LoadScene(nextSceneIndex);

        doLyricsCoroutine = null;
    }

    private void UpdateLyricsDisplay() {
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

    private void ResetLyricsProgressBar() {
        lyricsProgressBarFill.sizeDelta = new Vector2(0, lyricsProgressBarFill.sizeDelta.y);
    }

    private void ClearLyrics() {
        lyricsDisplay.text = "";
        lyricsBackground.SetActive(false);
        canvas.SetActive(false);
        SceneManager.LoadScene(nextSceneIndex);
    }
}
