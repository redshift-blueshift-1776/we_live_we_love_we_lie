using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Level8Intro : MonoBehaviour
{
    [Header("Cutscene Objects")]
    [SerializeField] public GameObject cam1;
    
    [Header("Square Rings")]
    [SerializeField] public GameObject SquareRingsReference;
    [SerializeField] public GameObject SquareRing;
    [SerializeField] public float ringOffset;
    [SerializeField] public float ringOffsetRotation;
    [SerializeField] public int numRings;
    [SerializeField] public int numRingsInRotation;

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
        GenerateSquareRings();

        lyrics = new List<string> {
            "Welcome to the FINAL challenge!",
            "After seven BRUTAL challenges, it all comes down to this...",
            "One FINAL challenge!",
            "Every action you've taken so far in these games has led to this moment...",
            "And you two are now one step away from getting the vacant spot in the game dev program.",
            "And for this final challenge... you will be playing Lan Attis's newest rhythm game.",
            "Good luck, contestants. This is the last step before getting into the game dev program.",
            "May the best player win."
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
        int usePostProcessing = PlayerPrefs.GetInt("useVisualEffects", 0);
        SquareRingsReference.transform.position += new Vector3(0, 0, -20) * Time.deltaTime * usePostProcessing;
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

    public void GenerateSquareRings() {
        for (int i = 0; i < numRings; i++) {
            GameObject newSquareRing = Instantiate(SquareRing);
            newSquareRing.transform.SetParent(SquareRingsReference.transform);
            newSquareRing.transform.localPosition = new Vector3(0, 0, i * ringOffset);
            newSquareRing.transform.localRotation = Quaternion.Euler(0, 0, i * ringOffsetRotation);
            SquareRing sr = newSquareRing.GetComponent<SquareRing>();
            sr.H = ((float) i) / numRingsInRotation;
        }
    }
}
