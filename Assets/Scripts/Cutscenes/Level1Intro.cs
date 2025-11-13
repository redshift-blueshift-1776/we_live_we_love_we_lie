using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Level1Intro : MonoBehaviour
{

    [SerializeField] public GameObject pillar;
    [SerializeField] public GameObject transition;

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
        MakeGrid();

        lyrics = new List<string> {
            "Welcome to Spectre Games!",
            "I'm your host, Donald James, head of the Game Development Program at Pen You Never City",
            "Competing for the one vacant spot in the Game Dev program are these ONE THOUSAND CONTESTANTS!",
            "We have some crazy challenges coming up...",
            "Contestants will have to drive at ridiculous speeds across the city...",
            "Memorize paths through a maze...",
            "Solve difficult puzzles...",
            "And even shoot guns at robots...",
            "But first...",
            "Is everyone ready for the FIRST CHALLENGE?",
            "",
            "We have a challenge that will likely eliminate a large part of the contestant pool...",
            "For the FIRST CHALLENGE, you will have to stack 10 layers of blocks...",
            "If you fail to do so within two minutes, or your tower falls, you are eliminated...",
            "This is just the FIRST of many challenges",
            "And there are more brutal challenges to come...",
            "Good luck, competitors... you're going to need it..."
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
    }

    public void MakeGrid() {
        // Make a square grid of size gridSize with squares of side squareSize
        // clear old children if rerun
        // foreach (Transform child in transform)
        // {
        //     Destroy(child.gameObject);
        // }

        // Center grid at (0,0)
        for (int x = 0; x < 20; x++)
        {
            for (int z = 0; z < 50; z++)
            {
                GameObject square = Instantiate(pillar);
                square.transform.SetParent(transform);

                square.transform.localScale = new Vector3(1f, 1f, 1f);

                float worldX = x * 20 - 190;
                float worldZ = z * 20;

                square.transform.localPosition = new Vector3(worldX, 0f, worldZ);
                EliminationPillar ep = square.GetComponent<EliminationPillar>();
                ep.transition = transition;
                ep.eliminate = false;
                ep.drop = false;
            }
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
